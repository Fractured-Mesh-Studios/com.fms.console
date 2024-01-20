using ConsoleEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ConsoleEditor
{
    [CustomEditor(typeof(Console), true)]
    public class ConsoleEditor : WindowEditor
    {
        private int m_toolBar;
        private GUIContent[] m_toolBarContent;
        private Console m_console;

        protected bool m_iconTab;
        protected bool m_consoleTab;
        protected bool m_consoleRenderTab;
        protected bool m_consoleCommandTab;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_console = (Console)m_target;
            m_toolBarContent = new GUIContent[2];
            m_toolBarContent[0] = EditorGUIUtility.IconContent("Canvas Icon");
            m_toolBarContent[1] = EditorGUIUtility.IconContent("d_winbtn_win_max@2x");
            //m_toolBarContent[1] = EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow@x2");
        }

        public override void OnInspectorGUI()
        {
            DrawWindow();
            DrawConsole();

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawConsole()
        {
            Tab(ref m_consoleTab, "Console", () =>
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("time"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("collapse"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useIcons"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("textMaxLength"));
            });

            Tab(ref m_consoleRenderTab, "Console Render", () =>
            {
                GUILayout.Label("Icons", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_timeIcon"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_infoIcon"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_warningIcon"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_errorIcon"));
                GUILayout.Space(10);
                GUILayout.Label("Colors", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_boxColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_buttonColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_scrollColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_textAreaColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_textFieldColor"));
            });

            Tab(ref m_consoleCommandTab, "Console Commands", () => {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("commands"));
            });

            DrawButtons();

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear")) { m_console.Clear(); }
            if (GUILayout.Button("Setup")) { m_target.gameObject.layer = LayerMask.NameToLayer("UI"); }
            EditorGUILayout.EndHorizontal();

        }
    }
}
