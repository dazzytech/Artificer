using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Base class for each UI
/// Panel. 
/// collapsable and responive to mouse events/draggable
/// </summary>
public class HUDPanel : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler,
    IDragHandler
{
    #region ENUM

    private enum WindowType { NONCOMBAT, COMBAT, NONE };

    #endregion

    #region ATTRIBUTES

    private bool m_collapseDelay = false;

    private bool m_hidden = false;

    private bool m_combatActive = false;

    private bool m_editMode = false;

    [Header("Base Panel")]

    /// <summary>
    /// Whether or not the window may be dragged
    /// </summary>
    [SerializeField]
    private bool m_editable = false;

    /// <summary>
    /// The id of the key within the controls
    /// alibrary
    /// </summary>
    [SerializeField]
    private string m_keyIndex;

    /// <summary>
    /// How does the window respond to the 
    /// ship combat state (e.g. storage would disappear)
    /// </summary>
    [SerializeField]
    private WindowType m_windowType;

    #region HUD ELEMENTS

    /// <summary>
    /// Entire Window
    /// </summary>
    [SerializeField]
    protected Transform m_HUD;

    /// <summary>
    /// Header bar used for draggin etc
    /// </summary>
    [SerializeField]
    private Transform m_header;

    /// <summary>
    /// Area that collapses and expands
    /// </summary>
    [SerializeField]
    private Transform m_body;

    /// <summary>
    /// The image that responds to
    /// colour changes based on mouse
    /// </summary>
    [SerializeField]
    private RawImage m_background;

    #endregion

    #region COLOURS

    [Header("Colours")]

    // Colour when mouse is not over
    [SerializeField]
    private Color m_idleColour = 
        new Color(0.65f, 0.67f, 0.84f, 0.10f);

    // Colour when mouse is hovering
    [SerializeField]
    private Color m_hoverColour = 
        new Color(0.65f, 0.77f, 0.84f, 0.30f);

    #endregion

    #endregion

    #region ACCESSOR

    /// <summary>
    /// Key used to collapse/expand window
    /// </summary>
    public KeyCode Key
    {
        get { return Control_Config.GetKey(m_keyIndex, "sys"); }
    }

    /// <summary>
    /// Used to determine if to hide or not
    /// </summary>
    public bool Hidden
    {
        get { return !m_HUD.gameObject.activeSelf; }
    }

    /// <summary>
    /// Set the ability to change the windows
    /// </summary>
    public bool EditMode
    {
        set
        {
            if (m_combatActive || !m_editable)
                m_editMode = false;
            else
                m_editMode = value;
        }
    }

    #endregion

    #region MONO BEHAVIOUR

    private void OnEnable()
    {
        if(m_background != null)
            m_background.color = m_idleColour;
    }

    #endregion

    #region PUBLIC INTERACTION

    /// <summary>
    /// Hides the body of the window 
    /// leaving only the header
    /// </summary>
    /// <param name="hide"></param>
    public void ToggleHUD(bool hide)
    {
        if (!m_collapseDelay && !m_hidden)
        {
            m_body.gameObject.SetActive(hide);
            m_collapseDelay = true;
            Invoke("PauseRelease", 0.3f);
        }
    }

    #endregion

    #region PRIVATE UTILITIES

    /// <summary>
    /// Detects if the player moved in or
    /// out of combat state and updates
    /// window visibility based on type
    /// </summary>
    /// <param name="newState"></param>
    private void DetectState(bool newState)
    {
        switch(m_windowType)
        {
            case WindowType.NONCOMBAT:
                m_body.gameObject.SetActive(!newState);
                m_hidden = newState;
                break;
            case WindowType.COMBAT:
                m_body.gameObject.SetActive(newState);
                m_hidden = !newState;
                break;
                // Dont do anything for none
        }

        m_combatActive = newState;
        if (m_combatActive)
        {
            m_background.color = m_idleColour;
            m_editable = false;
        }
    }

    /// <summary>
    /// Allows the window to collapse 
    /// or expand after alloted time
    /// </summary>
    private void PauseRelease()
    {
        m_collapseDelay = false;
    }

    #endregion

    #region POINTER EVENT DATA

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!m_combatActive)
            if (!(m_background == null))
                m_background.color = m_hoverColour;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!(m_background == null))
            m_background.color = m_idleColour;
    }

    public void OnDrag(PointerEventData data)
    {
        if (!m_editable || m_combatActive || !m_editMode)
            return;

            // can drag to better position     
            Vector2 currentPosition = transform.position;
            currentPosition.x += data.delta.x;
            currentPosition.y += data.delta.y;

            float parentLeft =
            (transform.parent.position.x -
            (transform.parent.GetComponent<RectTransform>().rect.width * .5f)
            + (GetComponent<RectTransform>().rect.width * .5f));

            float parentRight =
                (transform.parent.position.x +
                (transform.parent.GetComponent<RectTransform>().rect.width * .5f))
                - (GetComponent<RectTransform>().rect.width * .5f);

            float parentBottom = (transform.parent.position.y -
                (transform.parent.GetComponent<RectTransform>().rect.height * .5f))
                + (GetComponent<RectTransform>().rect.height * .5f);

            float parentTop = (transform.parent.position.y +
                (transform.parent.GetComponent<RectTransform>().rect.height * .5f))
                - (GetComponent<RectTransform>().rect.height * .5f);

            currentPosition.x = Mathf.Clamp(currentPosition.x, parentLeft, parentRight);
            currentPosition.y = Mathf.Clamp(currentPosition.y, parentBottom, parentTop);

            transform.position = currentPosition;
    }

    #endregion
}