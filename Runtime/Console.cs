using System;
using System.Collections;

using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ConsoleEngine
{
    public class Console : Window
    {
        private const int MAX_ICON_HEIGHT = 38;

        [HideInInspector] public bool time;
        [HideInInspector] public bool collapse = true;
        [HideInInspector] public bool useIcons = true;

        [SerializeField, HideInInspector] private Texture2D m_timeIcon;
        [SerializeField, HideInInspector] private Texture2D m_scaleIcon;
        [SerializeField, HideInInspector] private Texture2D m_infoIcon;
        [SerializeField, HideInInspector] private Texture2D m_warningIcon;
        [SerializeField, HideInInspector] private Texture2D m_errorIcon;

        [SerializeField, HideInInspector] private Color m_boxColor = Color.white;
        [SerializeField, HideInInspector] private Color m_buttonColor = Color.white;
        [SerializeField, HideInInspector] private Color m_scrollColor = Color.white;
        [SerializeField, HideInInspector] private Color m_textAreaColor = Color.white;
        [SerializeField, HideInInspector] private Color m_textFieldColor = Color.white;

        private List<ConsoleData> m_log = new List<ConsoleData>();
        private List<ConsoleData> m_filter = new List<ConsoleData>();

        private Vector2 m_scroll;
        private int m_filterState = 0;
        private bool m_info = true;
        private bool m_warning = true;
        private bool m_error = true;

        #region UNITY
        protected override void OnEnable()
        {
            base.OnEnable();
            Application.logMessageReceivedThreaded += OnMessageReceived;

            //Window
            onOpen += OnOpenWindow;
            onClose += OnCloseWindow;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Application.logMessageReceivedThreaded -= OnMessageReceived;

            //Window
            onOpen -= OnOpenWindow;
            onClose -= OnCloseWindow;
        }
        #endregion

        #region GUI
        protected override void OnWindow(int id)
        {
            OnHeader();
            OnBody();
            OnFooter();
        }

        protected virtual void OnHeader()
        {
            GUILayout.Space(5);
            GUILayoutOption min = GUILayout.MinWidth(38);
            GUILayoutOption max = GUILayout.MaxHeight(MAX_ICON_HEIGHT);

            GUI.color = m_boxColor;
            GUILayout.BeginHorizontal(GUI.skin.box);
            GUI.color = m_buttonColor;
            if (GUILayout.Button("Clear", min, max)) { Clear(); }
            GUI.color = collapse ? m_buttonColor : Color.grey;
            if (GUILayout.Button("Colapse", min, max)) { collapse = !collapse; }
            GUI.color = time ? m_buttonColor : Color.grey;
            if (GUILayout.Button(m_timeIcon, max)) { time = !time; }
            GUILayout.FlexibleSpace();
            GUI.color = m_info ? m_buttonColor : Color.grey;
            if (GUILayout.Button(m_infoIcon, max)) { m_info = !m_info; m_filterState++; }
            GUI.color = m_warning ? m_buttonColor : Color.grey;
            if (GUILayout.Button(m_warningIcon, max)) { m_warning = !m_warning; m_filterState++; }
            GUI.color = m_error ? m_buttonColor : Color.grey;
            if (GUILayout.Button(m_errorIcon, max)) { m_error = !m_error; m_filterState++; }
            GUILayout.EndHorizontal();
        }

        protected virtual void OnBody()
        {
            if (m_filterState > 0)
            {
                m_filter.Clear();
                Filter();
                m_filterState = 0;
            }

            ConsoleData temp;
            GUI.color = m_scrollColor;
            m_scroll = GUILayout.BeginScrollView(m_scroll, GUI.skin.box);
            for (int i = 0; i < m_filter.Count; i++)
            {
                temp = new ConsoleData(m_filter[i]);

                if (collapse)
                {
                    if (m_filter.FindAll(x => x == temp).Count > 1)
                    {
                        m_filter.RemoveAll(x => x == temp);
                        m_filter.Insert(i, temp);
                    }

                    m_filter[i] = DrawLog(m_filter[i], useIcons);
                }
                else
                {
                    GUILayout.BeginVertical();
                    for (int a = 0; a < m_log[i].amount; a++)
                    {
                        m_filter[i] = DrawLog(m_filter[i], useIcons);
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndScrollView();
        }

        protected virtual void OnFooter()
        {
            
        }

        private ConsoleData DrawLog(ConsoleData data, bool useIcon = false)
        {
            GUI.color = Color.white;
            GUIContent content = new GUIContent();
            if (!useIcon)
            {
                content = new GUIContent(data.name, data.type.ToString());
            }
            else
            {
                switch (data.type)
                {
                    case LogType.Log:
                        content = new GUIContent(data.name, m_infoIcon, data.type.ToString());
                        break;
                    //
                    case LogType.Warning:
                        content = new GUIContent(data.name, m_warningIcon, data.type.ToString());
                        break;
                    //
                    case LogType.Error:
                    case LogType.Exception:
                    case LogType.Assert:
                        content = new GUIContent(data.name, m_errorIcon, data.type.ToString());
                        break;
                }
            }

            GUIStyle style = new GUIStyle("button");
            ConsoleData cache = new ConsoleData(data);
            GUI.color = m_buttonColor;

            style.alignment = TextAnchor.MiddleLeft;
            if (GUILayout.Button(content, style))
            {
                bool newValue = !cache.expanded;
                cache = new ConsoleData(
                    data.name,
                    data.stackTrace,
                    data.type,
                    data.amount,
                    newValue
                );
            }

            if (cache.expanded)
            {
                GUI.color = m_textAreaColor;
                GUILayout.TextArea(cache.stackTrace);
            }
            return cache;
        }
        #endregion

        public void Clear()
        {
            m_log.Clear();
            m_filter.Clear();
            Debug.ClearDeveloperConsole();
        }

        #region FILTER
        private void Filter()
        {
            if (!m_error)
            {
                m_filter.RemoveAll(x => x.type == LogType.Error);
                m_filter.RemoveAll(x => x.type == LogType.Exception);
                m_filter.RemoveAll(x => x.type == LogType.Assert);
            }
            else
            {
                m_filter.AddRange(Filter(LogType.Error));
                m_filter.AddRange(Filter(LogType.Exception));
                m_filter.AddRange(Filter(LogType.Assert));
            }

            if (!m_info)
            {
                m_filter.RemoveAll(x => x.type == LogType.Log);
            }
            else
            {
                m_filter.AddRange(Filter(LogType.Log));
            }

            if (!m_warning) 
            {
                m_filter.RemoveAll(x => x.type == LogType.Warning);
            }
            else
            {
                m_filter.AddRange(Filter(LogType.Warning));
            }
        }

        private ConsoleData[] Filter(LogType logType)
        {
            List<ConsoleData> result = new List<ConsoleData>();

            for(int i = 0; i < m_log.Count; i++)
            {
                if (m_log[i].type == logType)
                {
                    result.Add(m_log[i]);
                }
            }

            return result.ToArray();
        }
        #endregion

        #region CALLBACK
        private void OnMessageReceived(string condition, string stackTrace, LogType type)
        {
            ConsoleData data;
            data = new ConsoleData(condition, stackTrace, type);
            string name = time ? $"[{data.time}] {condition}" : condition;
            data = new ConsoleData(name, stackTrace, type);

            if(!m_log.Contains(data))
            {
                m_log.Add(data);
            }
            else
            {
                int index = m_log.FindIndex(x => x == data);
                if(index >= 0 && index < m_log.Count) 
                {
                    int value = m_log[index].amount;
                    data.amount = value + 1;
                    m_log[index] = data;
                }
            }
        }

        private void OnOpenWindow(bool value) 
        {
            m_filter = new List<ConsoleData>(m_log);
        }

        private void OnCloseWindow(bool value) 
        { 
            m_filter.Clear();
        }

        #endregion

    }
}
