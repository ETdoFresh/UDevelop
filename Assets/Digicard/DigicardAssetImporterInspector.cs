using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DigicardAssetImporter))]
 public sealed class DigicardAssetImporterInspector : Editor
 {
     SerializedProperty m_name;
     SerializedProperty type;
     SerializedProperty properties;
     SerializedProperty coverArt;
     SerializedProperty description;

     private void OnEnable()
     {
         m_name      = serializedObject.FindProperty("cardName");
         type        = serializedObject.FindProperty("cardType");
         properties  = serializedObject.FindProperty("cardProperties");
         description = serializedObject.FindProperty("cardDescription");
         coverArt    = serializedObject.FindProperty("cardCoverArt");
     }

     public override void OnInspectorGUI()
     {
         DrawImporterGUI();
         base.serializedObject.ApplyModifiedProperties();
     }

     private void DrawImporterGUI()
     {
         //TODO: Create Property Fields of all Digicard Asset fields.
         EditorGUILayout.PropertyField(m_name,       new GUIContent("Card Name"));
         EditorGUILayout.PropertyField(type,         new GUIContent("Card Type"));
         EditorGUILayout.PropertyField(properties,   new GUIContent("Card Properties"), true);
         EditorGUILayout.PropertyField(coverArt,     new GUIContent("Card Cover Art"));
         EditorGUILayout.PropertyField(description,  new GUIContent("Card Description"));

         EditorGUILayout.Space(2);
         
         if(GUILayout.Button("Save"))
         {
             var importer = (serializedObject.targetObject as DigicardAssetImporter);
             importer.Save();
         }
     }
 }