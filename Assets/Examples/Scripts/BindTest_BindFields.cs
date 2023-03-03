using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
[RequireComponent(typeof(UnityEngine.UIElements.UIDocument))]
public partial class BindTest : MonoBehaviour
{
    public UnityEngine.UIElements.Label Title { get; private set; }
    public UnityEngine.UIElements.Button Button1 { get; private set; }

    public void Bind()
    {
        VisualElement root = gameObject.GetComponent<UIDocument>().rootVisualElement;
        Title = root.Q<UnityEngine.UIElements.Label>("Title");
        Button1 = root.Q<UnityEngine.UIElements.Button>("Button1");
    }
}
