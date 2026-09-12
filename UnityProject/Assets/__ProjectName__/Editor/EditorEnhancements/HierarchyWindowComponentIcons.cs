#if UNITY_6000_5_OR_NEWER

using Unity.Hierarchy;
using Unity.Hierarchy.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tenebrous.EditorEnhancements
{
    internal static class HierarchyWindowComponentIcons
    {
        private const string ColumnId = "Tenebrous/Components";

        [HierarchyViewColumnDescriptor(ColumnId)]
        private static void CreateColumnDesc(HierarchyViewColumnDescriptor desc)
        {
            desc.Title = "Components";
            desc.Tooltip = "Attached components";
            desc.DefaultWidth = 120;
            desc.DefaultVisibility = true;
        }

        [HierarchyViewCellDescriptor(ColumnId, typeof(HierarchyGameObjectHandler))]
        private static void CreateCellDesc(HierarchyViewCellDescriptor desc)
        {
            desc.BindCell = BindCell;
        }

        private static void BindCell(HierarchyViewCell cell)
        {
            if( cell.Handler is not HierarchyGameObjectHandler handler )
                return;

            GameObject gameObject = handler.GetGameObject( cell.Node );
            if( gameObject == null )
                return;

            cell.style.flexDirection = FlexDirection.Row;
            cell.style.justifyContent = Justify.FlexEnd;
            cell.IsDefaultValue = false;

            foreach( Component component in gameObject.GetComponents<Component>() )
            {
                if( component is Transform )
                    continue;

                Texture icon = GetComponentIcon( component );
                if( icon == null )
                    continue;

                Image image = new Image();
                image.image = icon;
                image.style.width = 16;
                image.style.height = 16;
                image.tintColor = component.GetEnabled() ? Color.white : new Color( 1.0f, 1.0f, 1.0f, 0.5f );

                Component capturedComponent = component;
                image.RegisterCallback<ClickEvent>( evt => {
                    capturedComponent.SetEnabled( !capturedComponent.GetEnabled() );
                    image.tintColor = capturedComponent.GetEnabled() ? Color.white : new Color( 1.0f, 1.0f, 1.0f, 0.5f );
                    evt.StopPropagation();
                } );

                cell.Add( image );
            }
        }

        private static Texture GetComponentIcon( Component component )
        {
            if( component is MonoBehaviour monoBehaviour )
            {
                MonoScript script = MonoScript.FromMonoBehaviour( monoBehaviour );
                Texture icon = AssetDatabase.GetCachedIcon( AssetDatabase.GetAssetPath( script ) );

                if( icon != null )
                    return icon;
            }

            return AssetPreview.GetMiniThumbnail( component );
        }
    }
}

#endif
