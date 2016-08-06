using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PanelMouseHighlight : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    #region ATTRIBUTES

    public Color NormalColour;
    public Color HighlightColour;

    private Image _rect;

    #endregion

    #region MONO BEHAVIOUR

    void Awake()
    {
      //  _rect = GetComponent<Image>();
    }

    #endregion
}
