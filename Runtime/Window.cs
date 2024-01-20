using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ConsoleEngine
{
    [ExecuteInEditMode]
    public class Window : MonoBehaviour
    {
        private const float RESIZE_WIDTH = 20;
        private const float RESIZE_HEIGHT = 20;

        [HideInInspector] public Rect rect = new Rect(50, 50, 400, 200);
        [HideInInspector] public Vector2 dragSize = new Vector2(400, 20);

        [HideInInspector] public string title = "Window";
        [HideInInspector] public string tooltip = "Ingame Window";
        [HideInInspector] public Texture icon = null;
        [HideInInspector] public Texture resizeIcon = null;
        [HideInInspector] public bool drag, resize, clamp;
        [HideInInspector] public Color color = Color.white;
        [HideInInspector][Range(0,1)] public float opacity = 1f;
        
        [SerializeField, HideInInspector] private GUISkin m_skin;

        public Action<bool> onOpen;
        public Action<bool> onClose;

        public bool isOpen => m_isOpen;

        private GUIContent m_windowContent;
        private int m_windowId;
        private bool m_isOpen;
        private Event m_event;

        private Vector2 m_mousePosition, m_mouseDragPosition;
        private Rect m_dragRect;
        private Rect m_resizeRect;
        private Rect m_closeRect;
        private bool m_canDrag = false;
        private bool m_canResize = false;

        #region UNITY
        protected virtual void OnEnable()
        {
            m_windowContent = new GUIContent(title, icon, tooltip);
        }

        protected virtual void OnDisable()
        {
            m_windowContent = new GUIContent();
            m_isOpen = false;
        }
        #endregion

        #region INPUT
        private void OnToggle(InputValue value)
        {
            if (value.isPressed)
            {
                Toggle();
            }
        }
        #endregion

        #region GUI
        private void OnGUI()
        {
            GUI.skin = m_skin;
            m_event = Event.current;

            Vector2 resizeOffset = new Vector2(
                rect.width - RESIZE_WIDTH,
                rect.height - RESIZE_HEIGHT
            );

            if(m_isOpen)
            {
                m_mousePosition = Event.current.mousePosition;
                m_dragRect = new Rect(rect.position, dragSize);
                m_resizeRect = new Rect(rect.position + resizeOffset, new Vector2(RESIZE_WIDTH, RESIZE_HEIGHT));

                if(m_event.type == EventType.MouseDown && m_event.button == 0)
                {
                    //Resize
                    m_canResize = m_resizeRect.Contains(m_mousePosition);

                    //Drag
                    if (m_dragRect.Contains(m_mousePosition))
                    {
                        m_canDrag = true;
                        m_mouseDragPosition = m_mousePosition - rect.position;
                    }
                    else
                    {
                        m_canDrag = false;
                    }

                    m_event.Use();
                }

                m_canDrag = drag ? m_canDrag : false;
                m_canResize = resize ? m_canResize : false;

                if(resize) GUI.Label(m_resizeRect, resizeIcon);

                if(m_event.type == EventType.MouseDrag && m_event.button == 0 && m_canDrag)
                {
                    rect = new Rect(m_mousePosition - m_mouseDragPosition, rect.size);

                    if (clamp)
                    {
                        rect.x = Mathf.Clamp(rect.x, 0, Screen.width - rect.width);
                        rect.y = Mathf.Clamp(rect.y, 0, Screen.height - rect.height);
                    }
                }

                if(m_event.type == EventType.MouseDrag && m_event.button == 0 && m_canResize)
                {
                    var w = m_mousePosition.x - rect.x;
                    var h = m_mousePosition.y - rect.y;

                    rect.width = Mathf.Clamp(w, Screen.width * 0.1f, Screen.width);
                    rect.height = Mathf.Clamp(h, Screen.height * 0.1f, Screen.height);

                    m_dragRect.width = rect.width;
                    m_dragRect.height = rect.height;
                }

                /*m_closeRect = new Rect(rect.x + rect.width - 40, rect.y, 40, 20);
                if (GUI.Button(m_closeRect, "X"))
                {
                    Close();
                }*/

                //Window
                GUI.color = new Color(color.r,color.g, color.b, opacity);
                rect = GUI.Window(m_windowId, rect, OnWindow, m_windowContent);
                GUI.color = Color.white;
            }
        }
        #endregion

        #region WINDOW
        protected virtual void OnWindow(int id)
        {

        }

        public void Open()
        {
            if (!m_isOpen)
            {
                m_isOpen = true;
                if (onOpen != null)
                    onOpen.Invoke(m_isOpen);
            }
        }

        public void Close()
        {
            if (m_isOpen)
            {
                m_isOpen = false;
                if (onClose != null)
                    onClose.Invoke(m_isOpen);
            }
        }

        public void Toggle()
        {
            m_isOpen = !m_isOpen;
            if (m_isOpen)
            {
                if(onClose!=null)
                    onClose.Invoke(m_isOpen);
            }
            else
            {
                if (onOpen != null)
                    onOpen.Invoke(m_isOpen);
            }
        }
        #endregion
    }
}