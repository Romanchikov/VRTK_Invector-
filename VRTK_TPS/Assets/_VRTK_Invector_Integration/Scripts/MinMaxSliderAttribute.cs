using UnityEngine;
using UnityEditor;

public class MinMaxSliderAttribute : PropertyAttribute
{

    public readonly float max;
    public readonly float min;

    public MinMaxSliderAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}



[CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
class MinMaxSliderDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            Vector2 range = property.vector2Value;
            float min = range.x;
            float max = range.y;
            MinMaxSliderAttribute attr = attribute as MinMaxSliderAttribute;
            EditorGUI.BeginChangeCheck();
            float horizontalSpace = 10; // in pixel
            float floatFieldWidth = 50; // in pixel
            Rect sliderPos = new Rect(position.position, new Vector2(position.width - horizontalSpace * 2 - floatFieldWidth * 2, position.height));
            Rect valueMinPos = new Rect(new Vector2(sliderPos.xMax + horizontalSpace, position.yMin), new Vector2(floatFieldWidth, position.height));
            Rect valueMaxPos = new Rect(new Vector2(valueMinPos.xMax + horizontalSpace, position.yMin), new Vector2(floatFieldWidth, position.height));

            EditorGUI.MinMaxSlider(sliderPos, label, ref min, ref max, attr.min, attr.max);
            min = EditorGUI.FloatField(valueMinPos, min);
            max = EditorGUI.FloatField(valueMaxPos, max);
            if (EditorGUI.EndChangeCheck())
            {
                range.x = min;
                range.y = max;
                property.vector2Value = range;
            }
        }
        else
        {
            EditorGUI.LabelField(position, label, "Use only with Vector2");
        }
    }

}