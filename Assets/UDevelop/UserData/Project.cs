using System;
using System.Collections.Generic;

namespace UDevelop.UserData
{
    [Serializable]
    public class Project
    {
        public string firstLayout { get; set; } // First scene/layout loaded when the game starts
        public string version { get; set; } // Version of UDevelop used to create project
        public ProjectProperties properties { get; set; } // Project properties 
        public List<Resource> resources { get; set; } // List of all resources 
        public List<Object> objects { get; set; } // List of all objects
        public List<ObjectGroup> objectsGroups { get; set; } // List of all objects groups
        public List<Variable> variables { get; set; } // List of all variables
        public List<Layout> layouts { get; set; } // List of all scenes
        public List<Event> externalEvents { get; set; } // List of all external events
        public List<EventsFunctionsExtension> eventsFunctionsExtensions { get; set; } // List of all extensions
        public List<Layout> externalLayouts { get; set; } // List of all external scenes
        public List<object> externalSourceFiles { get; set; } // List of all external source files
    }
    
    public class ProjectProperties
    {
        public bool adaptGameResolutionAtRuntime { get; set; }
        public string antialiasingMode { get; set; }
        public bool antialisingEnabledOnMobile { get; set; }
        public bool folderProject { get; set; }
        public string orientation { get; set; }
        public string packageName { get; set; }
        public bool pixelsRounding { get; set; }
        public string projectUuid { get; set; }
        public string scaleMode { get; set; }
        public string sizeOnStartupMode { get; set; }
        public string templateSlug { get; set; }
        public bool useExternalSourceFiles { get; set; }
        public string version { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string author { get; set; }
        public int windowWidth { get; set; }
        public int windowHeight { get; set; }
        public string latestCompilationDirectory { get; set; }
        public int maxFPS { get; set; }
        public int minFPS { get; set; }
        public bool verticalSync { get; set; }
        //public PlatformSpecificAssets platformSpecificAssets { get; set; }
        //public LoadingScreen loadingScreen { get; set; }
        //public Watermark watermark { get; set; }
        public List<object> authorIds { get; set; }
        public List<object> authorUsernames { get; set; }
        public List<string> categories { get; set; }
        public List<string> playableDevices { get; set; }
        public List<object> extensionProperties { get; set; }
        //public List<Platform> platforms { get; set; }
        public string currentPlatform { get; set; }
    }
    
    public class Resource
    {
        public bool alwaysLoaded { get; set; }
        public string file { get; set; }
        public string kind { get; set; }
        public string metadata { get; set; }
        public string name { get; set; }
        public bool smoothed { get; set; }
        public bool userAdded { get; set; }
        public bool? preloadAsMusic { get; set; }
        public bool? preloadAsSound { get; set; }
        public bool? preloadInCache { get; set; }
        //public Origin origin { get; set; }
        public List<Resource> resources { get; set; }
        public List<object> resourceFolders { get; set; }
    }
    
    public class ObjectGroup
    {
        public string name { get; set; }
        public List<Object> objects { get; set; }
    }
    
    public class Variable
    {
        public bool folded { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }
    
    public class Layout
    {
        public int b { get; set; }
        public bool disableInputWhenNotFocused { get; set; }
        public string mangledName { get; set; }
        public string name { get; set; }
        public int r { get; set; }
        public bool standardSortMethod { get; set; }
        public bool stopSoundsOnStartup { get; set; }
        public string title { get; set; }
        public int v { get; set; }
        //public UiSettings uiSettings { get; set; }
        public List<ObjectGroup> objectsGroups { get; set; }
        public List<Variable> variables { get; set; }
        public List<Instance> instances { get; set; }
        public List<Object> objects { get; set; }
        public List<Event> events { get; set; }
        public List<Layer> layers { get; set; }
        public List<BehaviorsSharedDatum> behaviorsSharedData { get; set; }
    }
    
    public class BehaviorsSharedDatum
    {
        public string name { get; set; }
        public string type { get; set; }
        public string ExternalUrls { get; set; }
    }
    
    public class Instance
    {
        public int angle { get; set; }
        public bool customSize { get; set; }
        public int depth { get; set; }
        public int height { get; set; }
        public string layer { get; set; }
        public string name { get; set; }
        public string persistentUuid { get; set; }
        public int width { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int zOrder { get; set; }
        public List<object> numberProperties { get; set; }
        public List<object> stringProperties { get; set; }
        public List<object> initialVariables { get; set; }
        public bool? locked { get; set; }
        public bool? @sealed { get; set; }
    }
    
    public class Event
    {
        public string type { get; set; }
        public List<Condition> conditions { get; set; }
        public List<Action> actions { get; set; }
        public bool? disabled { get; set; }
        public int? colorB { get; set; }
        public int? colorG { get; set; }
        public int? colorR { get; set; }
        public int? creationTime { get; set; }
        public string name { get; set; }
        public string source { get; set; }
        public List<Event> events { get; set; }
        public List<object> parameters { get; set; }
        public Color color { get; set; }
        public string comment { get; set; }
        public string repeatExpression { get; set; }
        public bool? folded { get; set; }
        public object inlineCode { get; set; }
        public string parameterObjects { get; set; }
        public bool useStrict { get; set; }
        public bool eventsSheetExpanded { get; set; }
        public bool? infiniteLoopWarning { get; set; }
        public List<WhileCondition> whileConditions { get; set; }
    }
    
    public class Color
    {
        public int b { get; set; }
        public int g { get; set; }
        public int r { get; set; }
        public int textB { get; set; }
        public int textG { get; set; }
        public int textR { get; set; }
    }
    
    public class Condition
    {
        public Type type { get; set; }
        public List<string> parameters { get; set; }
        public List<SubInstruction> subInstructions { get; set; }
    }
    
    public class Action
    {
        public Type type { get; set; }
        public List<string> parameters { get; set; }
    }
    
    public class WhileCondition
    {
        public Type type { get; set; }
        public List<string> parameters { get; set; }
    }
    
    public class SubInstruction
    {
        public Type type { get; set; }
        public List<string> parameters { get; set; }
    }
    
    public class Layer
    {
        public int ambientLightColorB { get; set; }
        public int ambientLightColorG { get; set; }
        public int ambientLightColorR { get; set; }
        public int camera3DFarPlaneDistance { get; set; }
        public int camera3DFieldOfView { get; set; }
        public double camera3DNearPlaneDistance { get; set; }
        public bool followBaseLayerCamera { get; set; }
        public bool isLightingLayer { get; set; }
        public bool isLocked { get; set; }
        public string name { get; set; }
        public string renderingType { get; set; }
        public bool visibility { get; set; }
        public List<Camera> cameras { get; set; }
        public List<Effect> effects { get; set; }
    }
    
    public class Camera
    {
        public bool defaultSize { get; set; }
        public bool defaultViewport { get; set; }
        public int height { get; set; }
        public int viewportBottom { get; set; }
        public int viewportLeft { get; set; }
        public int viewportRight { get; set; }
        public int viewportTop { get; set; }
        public int width { get; set; }
    }
    
    public class Effect
    {
        public string effectType { get; set; }
        public string name { get; set; }
        public DoubleParameters doubleParameters { get; set; }
        public StringParameters stringParameters { get; set; }
        public BooleanParameters booleanParameters { get; set; }
    }
    
    public class StringParameters
    {
        public string color { get; set; }
        public string groundColor { get; set; }
        public string skyColor { get; set; }
        public string top { get; set; }
    }
    
    public class DoubleParameters
    {
        public double alpha { get; set; }
        public double blur { get; set; }
        public int distance { get; set; }
        public int padding { get; set; }
        public int quality { get; set; }
        public int rotation { get; set; }
        public double? blue { get; set; }
        public int? brightness { get; set; }
        public int? contrast { get; set; }
        public int? gamma { get; set; }
        public double? green { get; set; }
        public double? red { get; set; }
        public int? saturation { get; set; }
        public int? thickness { get; set; }
        public int? pixelizeX { get; set; }
        public int? pixelizeY { get; set; }
        public int elevation { get; set; }
        public int intensity { get; set; }
    }
    
    public class BooleanParameters
    {
        public bool shadowOnly { get; set; }
    }
    
    public class EventsFunctionsExtension
    {
        public string author { get; set; }
        public string category { get; set; }
        public string extensionNamespace { get; set; }
        public string fullName { get; set; }
        public string helpPath { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string previewIconUrl { get; set; }
        public string shortDescription { get; set; }
        public string version { get; set; }
        public object description { get; set; }
        //public Origin origin { get; set; }
        public List<string> tags { get; set; }
        public List<string> authorIds { get; set; }
        public List<Dependency> dependencies { get; set; }
        public List<EventsFunction> eventsFunctions { get; set; }
        public List<EventsBasedBehavior> eventsBasedBehaviors { get; set; }
        public List<EventsBasedObject> eventsBasedObjects { get; set; }
    }
    
    public class Dependency
    {
        public string exportName { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string version { get; set; }
    }
    
    public class EventsFunction
    {
        public string description { get; set; }
        public string fullName { get; set; }
        public string functionType { get; set; }
        public string name { get; set; }
        public string sentence { get; set; }
        public List<Event> events { get; set; }
        public List<Parameter> parameters { get; set; }
        public List<object> objectGroups { get; set; }
        public ExpressionType expressionType { get; set; }
        public string group { get; set; }
        public bool? @private { get; set; }
        public string getterName { get; set; }
    }
    
    public class Parameter
    {
        public string description { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string supplementaryInformation { get; set; }
        public string longDescription { get; set; }
        public string defaultValue { get; set; }
        public bool? optional { get; set; }
    }
    
    public class ExpressionType
    {
        public string type { get; set; }
    }
    
    public class EventsBasedBehavior
    {
        public string description { get; set; }
        public string fullName { get; set; }
        public string name { get; set; }
        public string objectType { get; set; }
        public List<EventsFunction> eventsFunctions { get; set; }
        public List<PropertyDescriptor> propertyDescriptors { get; set; }
        public List<SharedPropertyDescriptor> sharedPropertyDescriptors { get; set; }
        public bool? @private { get; set; }
    }
    
    public class SharedPropertyDescriptor
    {
        public string value { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public string group { get; set; }
        public List<object> extraInformation { get; set; }
        public bool hidden { get; set; }
        public string name { get; set; }
    }
    
    public class PropertyDescriptor
    {
        public string value { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public string group { get; set; }
        public List<string> extraInformation { get; set; }
        public bool hidden { get; set; }
        public string name { get; set; }
        public string unit { get; set; }
    }
    
    public class EventsBasedObject
    {
        public string defaultName { get; set; }
        public string description { get; set; }
        public string fullName { get; set; }
        public string name { get; set; }
        public List<EventsFunction> eventsFunctions { get; set; }
        public List<PropertyDescriptor> propertyDescriptors { get; set; }
        public List<Object> objects { get; set; }
    }
}