using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MonsterAnimatorSetup
{
    private const string SourceFbxPath = "Assets/animation.fbx";
    private const string OutputFolder = "Assets/Animations";
    private const string ControllerPath = "Assets/Animations/Monster.controller";
    private const string MenuPath = "Tools/Monster/Setup Animator From animation.fbx";

    [MenuItem(MenuPath)]
    public static void SetupMonsterAnimator()
    {
        EnsureFolder(OutputFolder);

        AnimationClip[] clips = AssetDatabase
            .LoadAllAssetsAtPath(SourceFbxPath)
            .OfType<AnimationClip>()
            .Where(c => !c.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (clips.Length == 0)
        {
            // Fallback when importer only exposes preview-labeled clips.
            clips = AssetDatabase
                .LoadAllAssetsAtPath(SourceFbxPath)
                .OfType<AnimationClip>()
                .Where(c => !string.Equals(c.name, "__preview__", StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        if (clips.Length == 0)
        {
            Debug.LogError($"[MonsterAnimatorSetup] No animation clips found in {SourceFbxPath}");
            return;
        }

        AnimationClip idleClip = PickClip(clips, new[] { "idle", "wait", "stand" }) ?? clips[0];
        AnimationClip moveClip = PickClip(clips, new[] { "walk", "run", "move" }) ?? idleClip;
        AnimationClip attackClip = PickClip(clips, new[] { "attack", "hit", "punch", "kick" }) ?? moveClip;

        AnimatorController controller = BuildController(idleClip, moveClip, attackClip);
        if (controller == null)
        {
            Debug.LogError("[MonsterAnimatorSetup] Failed to create AnimatorController.");
            return;
        }

        Avatar avatar = AssetDatabase
            .LoadAllAssetsAtPath(SourceFbxPath)
            .OfType<Avatar>()
            .FirstOrDefault(a => a != null && a.isValid);

        int appliedCount = ApplyToMonstersInOpenScenes(controller, avatar);
        AssetDatabase.SaveAssets();

        if (appliedCount > 0)
        {
            EditorSceneManager.SaveOpenScenes();
        }

        Debug.Log($"[MonsterAnimatorSetup] Completed. Clips: idle={idleClip.name}, move={moveClip.name}, attack={attackClip.name}, applied={appliedCount}");
    }

    private static AnimatorController BuildController(AnimationClip idleClip, AnimationClip moveClip, AnimationClip attackClip)
    {
        AnimatorController existing = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (existing != null)
        {
            AssetDatabase.DeleteAsset(ControllerPath);
        }

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        if (controller == null)
        {
            return null;
        }

        controller.parameters = Array.Empty<AnimatorControllerParameter>();
        controller.AddParameter("IsMove", AnimatorControllerParameterType.Bool);
        controller.AddParameter("MoveSpeed", AnimatorControllerParameterType.Float);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);

        AnimatorControllerLayer layer = controller.layers[0];
        AnimatorStateMachine sm = layer.stateMachine;

        AnimatorState idle = sm.AddState("Idle");
        idle.motion = idleClip;

        AnimatorState move = sm.AddState("Move");
        move.motion = moveClip;

        AnimatorState attack = sm.AddState("Attack");
        attack.motion = attackClip;
        attack.writeDefaultValues = true;

        sm.defaultState = move;

        AnimatorStateTransition idleToMove = idle.AddTransition(move);
        idleToMove.hasExitTime = false;
        idleToMove.hasFixedDuration = true;
        idleToMove.duration = 0.1f;
        idleToMove.AddCondition(AnimatorConditionMode.If, 0f, "IsMove");

        AnimatorStateTransition moveToIdle = move.AddTransition(idle);
        moveToIdle.hasExitTime = false;
        moveToIdle.hasFixedDuration = true;
        moveToIdle.duration = 0.1f;
        moveToIdle.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsMove");

        AnimatorStateTransition anyToAttack = sm.AddAnyStateTransition(attack);
        anyToAttack.hasExitTime = false;
        anyToAttack.hasFixedDuration = true;
        anyToAttack.duration = 0.05f;
        anyToAttack.AddCondition(AnimatorConditionMode.If, 0f, "Attack");

        AnimatorStateTransition attackToIdle = attack.AddTransition(idle);
        attackToIdle.hasExitTime = true;
        attackToIdle.exitTime = 0.95f;
        attackToIdle.hasFixedDuration = true;
        attackToIdle.duration = 0.08f;

        EditorUtility.SetDirty(controller);
        return controller;
    }

    private static int ApplyToMonstersInOpenScenes(AnimatorController controller, Avatar avatar)
    {
        int applied = 0;
        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.IsValid())
        {
            return applied;
        }

        MonsterAI[] monsters = UnityEngine.Object.FindObjectsByType<MonsterAI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (MonsterAI monster in monsters)
        {
            if (monster == null)
            {
                continue;
            }

            Animator animator = monster.GetComponent<Animator>();
            if (animator == null)
            {
                animator = monster.gameObject.AddComponent<Animator>();
            }

            animator.runtimeAnimatorController = controller;
            if (avatar != null)
            {
                animator.avatar = avatar;
            }

            SerializedObject so = new SerializedObject(monster);
            SetObjectRef(so, "animator", animator);
            SetString(so, "moveBoolParameter", "IsMove");
            SetString(so, "moveSpeedParameter", "MoveSpeed");
            SetString(so, "attackTriggerParameter", "Attack");
            SetString(so, "idleStateName", "Idle");
            SetString(so, "moveStateName", "Move");
            SetString(so, "attackStateName", "Attack");
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(monster);
            EditorUtility.SetDirty(animator);
            applied++;
        }

        return applied;
    }

    private static void SetObjectRef(SerializedObject so, string propertyName, UnityEngine.Object value)
    {
        SerializedProperty property = so.FindProperty(propertyName);
        if (property != null)
        {
            property.objectReferenceValue = value;
        }
    }

    private static void SetString(SerializedObject so, string propertyName, string value)
    {
        SerializedProperty property = so.FindProperty(propertyName);
        if (property != null)
        {
            property.stringValue = value;
        }
    }

    private static AnimationClip PickClip(IEnumerable<AnimationClip> clips, IReadOnlyList<string> keywords)
    {
        foreach (AnimationClip clip in clips)
        {
            string lowerName = clip.name.ToLowerInvariant();
            for (int i = 0; i < keywords.Count; i++)
            {
                if (lowerName.Contains(keywords[i]))
                {
                    return clip;
                }
            }
        }

        return null;
    }

    private static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string[] tokens = folderPath.Split('/');
        if (tokens.Length < 2 || tokens[0] != "Assets")
        {
            throw new ArgumentException($"Folder path must start with Assets/: {folderPath}");
        }

        string current = "Assets";
        for (int i = 1; i < tokens.Length; i++)
        {
            string next = $"{current}/{tokens[i]}";
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, tokens[i]);
            }
            current = next;
        }
    }
}
