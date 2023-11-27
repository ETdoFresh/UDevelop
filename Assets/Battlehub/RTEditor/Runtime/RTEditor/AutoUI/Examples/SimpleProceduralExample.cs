using Battlehub.UIControls;
using Battlehub.UIControls.MenuControl;
using Battlehub.Utils;
using System;
using TMPro;
using UnityEngine;

namespace Battlehub.RTEditor.UI
{
    //[MenuDefinition]
    public static class AutoUISample
    {
        //[MenuCommand("Help/Auto UI Sample")]
        public static void Show()
        {
            AutoUI autoUI = new AutoUI();

            SimpleProceduralExample model = new SimpleProceduralExample();
            
            autoUI.CreateDialog(300, 400 , false, model, "SimpleProceduralExample");
        }
    }

    public class SimpleProceduralExample 
    {
        //Build UI with absolute positioning
        /*
        [Procedural]
        public static void BuildUI(AutoUI autoUI)
        {          
            var (image, _) = autoUI.Image(false);
            image.rectTransform.TopLeft(new Vector2(5, 0), new Vector2(185, 185));
            
            var (label, _) = autoUI.Label(false);
            label.rectTransform.TopLeft(new Vector2(5, -200), new Vector2(185, 185));
            
            image.sprite = Resources.Load<Sprite>("pizza-100");
            label.text = "Pine trees are evergreen, coniferous resinous trees (or, rarely, shrubs) growing 3–80 m (10–260 ft) tall, with the majority of species reaching 15–45 m (50–150 ft) tall. The smallest are Siberian dwarf pine and Potosi pinyon, and the tallest is an 81.79 m (268.35 ft) tall ponderosa pine.";
            label.alignment = TextAlignmentOptions.TopJustified;
            label.overflowMode = TextOverflowModes.Overflow;
        }
        */

        public class Target
        {
            private float m_value;
            public float Value
            {
                get { return m_value; }
                set
                {
                    if (value != m_value)
                    {
                        m_value = value;
                        Debug.Log(value);
                    }
                }
            }

            private Vector3 m_vector3Value;
            public Vector3 Vector3Value
            {
                get { return m_vector3Value; }
                set
                {
                    if (value != m_vector3Value)
                    {
                        m_vector3Value = value;
                        Debug.Log(value);
                    }
                }
            }
        }


        [Procedural]
        public static void BuildUI(AutoUI autoUI)
        {
            Target target = new Target();

            var verticalGroup = autoUI.BeginVerticalLayout(createLayoutElement:false, childForceExpandHeight: false);
            verticalGroup.Control.padding = new RectOffset(5, 5, 5, 5);

            var horizontalGroup = autoUI.BeginHorizontalLayout(createLayoutElement:true, childForceExpandWidth: false);
            horizontalGroup.LayoutElement.flexibleHeight = 0;
            
            var rangeEditor1 = autoUI.PropertyEditor<Range>(createLayoutElement: true);
            rangeEditor1.Control.SetRange(new Range(-1f, 1f));
            rangeEditor1.Control.Init(target, Strong.MemberInfo((Target x) => x.Value), "Label 1");

            var rangeEditor2 = autoUI.PropertyEditor<Range>(createLayoutElement:true);
            rangeEditor2.Control.SetRange(new Range(-1, 1));
            rangeEditor2.Control.Init(target, Strong.MemberInfo((Target x) => x.Value), "Label 2");

            autoUI.EndHorizontalLayout();

            var vectorEditor = autoUI.PropertyEditor<Vector3>(createLayoutElement: true);
            vectorEditor.Control.Init(target, Strong.MemberInfo((Target x) => x.Vector3Value), "Vector 3");

            // Labe absolute positioning
            var label = autoUI.Label(createLayoutElement:true);
            label.LayoutElement.ignoreLayout = true;
            label.Control.RectTransform().TopLeft(new Vector2(50, -100), new Vector2(185, 30));
            label.Control.text = "Label 3";

            // FloatEditor absolute positioning
            var floatEditor = autoUI.PropertyEditor<float>(createLayoutElement: true);
            floatEditor.LayoutElement.ignoreLayout = true;
            floatEditor.Control.RectTransform().TopLeft(new Vector2(50, -150), new Vector2(185, 30));
            floatEditor.Control.Init(target, Strong.MemberInfo((Target x) => x.Value), "Label 4");

            autoUI.EndVerticalLayout();
        }

        /*
        //Build UI using Layout elements
        [Procedural]        
        public static void BuildUI(AutoUI autoUI)
        {
            var (verticalGroup, _) = autoUI.BeginVerticalLayout();
            
            verticalGroup.childForceExpandWidth = false;
            verticalGroup.childForceExpandHeight = false;
            verticalGroup.spacing = 1;
            verticalGroup.padding = new RectOffset(5, 5, 5, 5);

            var (image, imageLayout) = autoUI.Image();
            imageLayout.preferredWidth = 200;
            imageLayout.preferredHeight = 200;
            imageLayout.flexibleHeight = 0;

            var (label, labelLayout) = autoUI.Label();
            labelLayout.flexibleHeight = 0;
            labelLayout.preferredHeight = 30;

            image.sprite = Resources.Load<Sprite>("pizza-100");
            label.text = "Pine trees are evergreen, coniferous resinous trees (or, rarely, shrubs) growing 3–80 m (10–260 ft) tall, with the majority of species reaching 15–45 m (50–150 ft) tall. The smallest are Siberian dwarf pine and Potosi pinyon, and the tallest is an 81.79 m (268.35 ft) tall ponderosa pine.";
            label.alignment = TextAlignmentOptions.TopJustified;
            label.overflowMode = TextOverflowModes.Overflow;
         
            autoUI.EndVerticalLayout();
        }
        */

        [DialogCancelAction("Close")]
        public void OnClose()
        {
            Debug.Log("Close");
        }
    }

}
