/*
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Tenebrous.EditorEnhancements
{
    [CustomPropertyDrawer( typeof( Texture2D ) )]
    public class Texture2DDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            EditorGUI.BeginProperty( position, label, property );
            var newValue = (Texture2D)EditorGUI.ObjectField( position, label, property.objectReferenceValue, typeof( Texture2D ), false );

            if( position.Contains( Event.current.mousePosition ) )
            {
                TeneEnhPreviewWindow.Update(
                    new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y,0,0),
                    Event.current.mousePosition,
                    pAsset : newValue
                );
            }

            EditorGUI.EndProperty();
        }
    }
}
*/