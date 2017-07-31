using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using UI.Effects;
using Space.Ship;

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

    /// <summary>
    /// None - Active in both states
    /// Noncombat - Active in none combat state
    /// Combat - Active in combat state
    /// </summary>
    private enum WindowType { NONE, NONCOMBAT, COMBAT };

    #endregion

    #region ATTRIBUTES

    #region EDIT ATTRIBUTES

    [Header("Edit")]
    [Header("Base Panel")]

    /// <summary>
    /// Whether or not the window may be dragged
    /// </summary>
    [SerializeField]
    private bool m_canEdit = false;

    /// <summary>
    /// edit mode allows the player to 
    /// start dragging and rearranging windows
    /// </summary>
    private bool m_editMode = false;

    #endregion

    #region COLLAPSABLE

    /// <summary>
    /// The id of the key within the controls
    /// to collapse the window
    /// </summary>
    [SerializeField]
    private string m_keyIndex;

    /// <summary>
    /// Stops the collapse ability for
    /// a short time
    /// </summary>
    private bool m_collapseDelay = false;

    /// <summary>
    /// Defined in editor if
    /// the player is able to minimize the window
    /// </summary>
    [SerializeField]
    private bool m_canCollapse = false;

    [SerializeField]
    private bool m_startMinimized = false;

    #endregion

    #region RESPONSIVE

    [Header("Responive")]

    /// <summary>
    /// How does the window respond to the 
    /// ship combat state (e.g. storage would disappear)
    /// </summary>
    [SerializeField]
    private WindowType m_windowType;

    /// <summary>
    /// Stores the state that the player is in
    /// </summary>
    private bool m_combatActive = false;

    /// <summary>
    /// Determines if the window
    /// responds at all to the player mouse
    /// </summary>
    [SerializeField]
    private bool m_responsive = false;

    #endregion

    #region HUD ELEMENTS

    [Header("HUD Elements")]

    /// <summary>
    /// Entire Window
    /// </summary>
    [SerializeField]
    protected Transform m_HUD;

    /// <summary>
    /// Header bar used for draggin etc
    /// </summary>
    [SerializeField]
    protected Transform m_header;

    /// <summary>
    /// Area that collapses and expands
    /// </summary>
    [SerializeField]
    protected Transform m_body;

    /// <summary>
    /// The image that responds to
    /// colour changes based on mouse
    /// </summary>
    [SerializeField]
    protected RawImage m_background;

    #endregion

    #region COLOURS

    [Header("Colours")]

    #region HIGHLIGHT

    /// <summary>
    /// Colour when mouse is not over
    /// </summary> 
    [SerializeField]
    private Color m_idleColour = 
        new Color(0.65f, 0.67f, 0.84f, 0.10f);

    /// <summary>
    /// Colour when mouse is hovering
    /// </summary> 
    [SerializeField]
    private Color m_hoverColour = 
        new Color(0.65f, 0.77f, 0.84f, 0.30f);

    #endregion

    #region EDITING

    /// <summary>
    /// Colour the HUD takes on when
    /// the player may edit it
    /// </summary>
    [SerializeField]
    private Color m_editColour;

    /// <summary>
    /// Colour when the mode is hovering the 
    /// object with mouse in edit mode
    /// </summary>
    [SerializeField]
    private Color m_editColourHover;

    /// <summary>
    /// COlour while player is draggin window
    /// </summary>
    [SerializeField]
    private Color m_editColourDrag;

    #endregion

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
        get { return !m_body.gameObject.activeSelf; }
    }

    /// <summary>
    /// Set the ability to change the windows
    /// </summary>
    public bool EditMode
    {
        set
        {
            if (!m_canEdit)
            {
                // if we are not able to actually edit 
                // the window the colour and disable edit mode
                if (m_background != null)
                    m_background.color = m_idleColour;
                m_editMode = false;
            }
            else
            {
                // set edit mode and colour based on
                // mode
                m_editMode = value;

                if(m_editMode)
                    if (m_background != null)
                        m_background.color = m_editColour;
                else
                    if (m_background != null)
                        m_background.color = m_idleColour;
            }
        }
    }

    #endregion

    #region MONO BEHAVIOUR

    protected virtual void OnEnable()
    {
        if(m_background != null)
            m_background.color = m_idleColour;

        // listen to the player state change
        ShipPlayerInputController.OnStateChanged += DetectState;

        if (m_startMinimized)
            ToggleHUD(false);
    }

    protected virtual void OnDisable()
    {
        ShipPlayerInputController.OnStateChanged -= DetectState;
    }

    #endregion

    #region PUBLIC INTERACTION

    /// <summary>
    /// Hides the body of the window 
    /// leaving only the header
    /// </summary>
    /// <param name="hide"></param>
    public virtual void ToggleHUD(bool hide)
    {
        if (!m_collapseDelay && m_canCollapse)
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
                break;
            case WindowType.COMBAT:
                m_body.gameObject.SetActive(newState);
                break;
                // Dont do anything for none
        }

        m_combatActive = newState;
        if (m_combatActive)
        {
            if(m_background != null)
                m_background.color = m_idleColour;
            m_canEdit = false;
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
            if (m_responsive && m_background != null)
                m_background.color = m_hoverColour;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_responsive && m_background != null)
        {
            if (m_editMode)
                    m_background.color = m_editColour;
                else
                    m_background.color = m_idleColour;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (!m_editMode)
            return;

        // can drag to better position     
        Vector2 currentPosition = transform.position;
        currentPosition.x += data.delta.x;
        currentPosition.y += data.delta.y;

        // constrict within bounds or parent
        transform.position = RectTransformExtension.RestrictBounds
            (GetComponent<RectTransform>(), transform.parent.GetComponent<RectTransform>(), 
            currentPosition);
    }

    #endregion
}