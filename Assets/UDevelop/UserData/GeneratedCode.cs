using System.Collections.Generic;

namespace UDevelop.UserData.GeneratedCode
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Action
    {
        public Type type { get; set; }
        public List<string> parameters { get; set; }
    }

    public class Animation
    {
        public string name { get; set; }
        public bool useMultipleDirections { get; set; }
        public List<Direction> directions { get; set; }
    }

    public class Background
    {
        public int bottomMargin { get; set; }
        public int height { get; set; }
        public int leftMargin { get; set; }
        public int rightMargin { get; set; }
        public string texture { get; set; }
        public bool tiled { get; set; }
        public int topMargin { get; set; }
        public int width { get; set; }
    }

    public class Behavior
    {
        public string name { get; set; }
        public string type { get; set; }
        public int Timer { get; set; }
        public string Color { get; set; }
        public string Type { get; set; }
        public string Direction { get; set; }
        public int MaxOpacity { get; set; }
        public int? ControllerIdentifier { get; set; }
        public string ButtonIdentifier { get; set; }
        public int? TouchId { get; set; }
        public int? TouchIndex { get; set; }
        public bool? IsReleased { get; set; }
        public string prefix { get; set; }
        public int? DecimalDigits { get; set; }
        public int? InteractableRadius { get; set; }
        public bool? IsInteractable { get; set; }
        public string JSON { get; set; }
        public string CurrentAnimation { get; set; }
        public double? CurrentAnimationFrame { get; set; }
        public string CurrentFrame { get; set; }
        public double? AnimationSpeed { get; set; }
        public string NameTagBackgroundColor { get; set; }
        public string DefaultColor { get; set; }
        public string DefaultOutlineColor { get; set; }
        public double? DrawChatWidth { get; set; }
        public double? DrawChatHeight { get; set; }
        public string SpriteSheetJSON { get; set; }
        public string InitialAnimationName { get; set; }
        public string JSONSpriteSheet { get; set; }
        public int? NumberOfDirections { get; set; }
        public string InteractorBehavior { get; set; }
        public string JoystickIdentifier { get; set; }
        public string TopDownMovement { get; set; }
        public int? GamepadIdentifier { get; set; }
        public bool? UseArrows { get; set; }
        public bool? UseLeftStick { get; set; }
        public bool? UseRightStick { get; set; }
        public string StickMode { get; set; }
        public int? acceleration { get; set; }
        public bool? allowDiagonals { get; set; }
        public int? angleOffset { get; set; }
        public int? angularMaxSpeed { get; set; }
        public int? customIsometryAngle { get; set; }
        public int? deceleration { get; set; }
        public bool? ignoreDefaultControls { get; set; }
        public int? maxSpeed { get; set; }
        public int? movementAngleOffset { get; set; }
        public bool? rotateObject { get; set; }
        public string viewpoint { get; set; }
        public string Property { get; set; }
        public double? Interval { get; set; }
        public bool? CharacterJustAdded { get; set; }
        public string CurrentCharacter { get; set; }
        public double? HalfPeriodTime { get; set; }
        public bool? IsFlashing { get; set; }
        public int? FlashDuration { get; set; }
        public int? Health { get; set; }
        public int? CurrentHealth { get; set; }
        public int? MaxHealth { get; set; }
        public int? DamageCooldown { get; set; }
        public bool? IsHealthJustDamaged { get; set; }
        public int? HealthRegenRate { get; set; }
        public int? HealthRegenDelay { get; set; }
        public bool? AllowOverHealing { get; set; }
        public bool? HitAtLeastOnce { get; set; }
        public bool? IsJustHealed { get; set; }
        public int? CurrentShieldPoints { get; set; }
        public int? MaxShieldPoints { get; set; }
        public int? ShieldDuration { get; set; }
        public int? ShieldRegenRate { get; set; }
        public bool? BlockExcessDamage { get; set; }
        public int? ShieldRegenDelay { get; set; }
        public bool? IsShieldJustDamaged { get; set; }
        public int? ChanceToDodge { get; set; }
        public int? DamageToBeApplied { get; set; }
        public int? FlatDamageReduction { get; set; }
        public int? PercentDamageReduction { get; set; }
        public bool? IsJustDodged { get; set; }
        public int? ShieldDamageTaken { get; set; }
        public int? HealToBeApplied { get; set; }
        public int Value { get; set; }
        public int MaxValue { get; set; }
        public int PreviousValue { get; set; }
        public bool? FloatingEnabled { get; set; }
        public double? DeadZoneRadius { get; set; }
        public int? JoystickAngle { get; set; }
        public int? JoystickForce { get; set; }
        public bool? ShouldCheckHovering { get; set; }
    }

    public class BehaviorsSharedDatum
    {
        public string name { get; set; }
        public string type { get; set; }
        public string ExternalUrls { get; set; }
    }

    public class BooleanParameters
    {
        public bool shadowOnly { get; set; }
    }

    public class Border
    {
        public bool adaptCollisionMaskAutomatically { get; set; }
        public bool updateIfNotVisible { get; set; }
        public List<Animation> animations { get; set; }
    }

    public class Buffer
    {
        public int bottomMargin { get; set; }
        public int height { get; set; }
        public int leftMargin { get; set; }
        public int rightMargin { get; set; }
        public string texture { get; set; }
        public bool tiled { get; set; }
        public int topMargin { get; set; }
        public int width { get; set; }
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

    public class CenterPoint
    {
        public bool automatic { get; set; }
        public string name { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class ChildrenContent
    {
        public Hovered Hovered { get; set; }
        public Idle Idle { get; set; }
        public Label Label { get; set; }
        public Pressed Pressed { get; set; }
        public Border Border { get; set; }
        public Thumb Thumb { get; set; }
        public Background Background { get; set; }
        public Buffer Buffer { get; set; }
        public FillBar FillBar { get; set; }
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

    public class Content
    {
        public int LeftPadding { get; set; }
        public int RightPadding { get; set; }
        public int PressedLabelOffsetY { get; set; }
        public int BottomPadding { get; set; }
        public int TopPadding { get; set; }
        public double HoveredFadeOutDuration { get; set; }
        public string tilemapJsonFile { get; set; }
        public string tilesetJsonFile { get; set; }
        public string collisionMaskTag { get; set; }
        public bool? debugMode { get; set; }
        public string fillColor { get; set; }
        public string outlineColor { get; set; }
        public int? fillOpacity { get; set; }
        public int? outlineOpacity { get; set; }
        public int? outlineSize { get; set; }
        public string tilemapAtlasImage { get; set; }
        public string displayMode { get; set; }
        public int? layerIndex { get; set; }
        public int? levelIndex { get; set; }
        public int? animationSpeedScale { get; set; }
        public int? animationFps { get; set; }
        public string text { get; set; }
        public int? opacity { get; set; }
        public double? scale { get; set; }
        public int? fontSize { get; set; }
        public string tint { get; set; }
        public string bitmapFontResourceName { get; set; }
        public string textureAtlasResourceName { get; set; }
        public string align { get; set; }
        public bool? wordWrap { get; set; }
        public int? IdleLabelOffsetY { get; set; }
        public int? MinimalWidth { get; set; }
        public int? PreviousHighValueDuration { get; set; }
        public bool? ShowLabel { get; set; }
        public int? BarTopPadding { get; set; }
        public int? BarBottomPadding { get; set; }
        public int? BarLeftPadding { get; set; }
        public int? BarRightPadding { get; set; }
        public int? MaxValue { get; set; }
        public int? InitialValue { get; set; }
    }

    public class Dependency
    {
        public string exportName { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string version { get; set; }
    }

    public class Direction
    {
        public bool looping { get; set; }
        public double timeBetweenFrames { get; set; }
        public List<Sprite> sprites { get; set; }
        public string metadata { get; set; }
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

    public class Effect
    {
        public string effectType { get; set; }
        public string name { get; set; }
        public DoubleParameters doubleParameters { get; set; }
        public StringParameters stringParameters { get; set; }
        public BooleanParameters booleanParameters { get; set; }
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
        public Origin origin { get; set; }
        public List<string> tags { get; set; }
        public List<string> authorIds { get; set; }
        public List<Dependency> dependencies { get; set; }
        public List<EventsFunction> eventsFunctions { get; set; }
        public List<EventsBasedBehavior> eventsBasedBehaviors { get; set; }
        public List<EventsBasedObject> eventsBasedObjects { get; set; }
    }

    public class ExpressionType
    {
        public string type { get; set; }
    }

    public class FillBar
    {
        public int bottomMargin { get; set; }
        public int height { get; set; }
        public int leftMargin { get; set; }
        public int rightMargin { get; set; }
        public string texture { get; set; }
        public bool tiled { get; set; }
        public int topMargin { get; set; }
        public int width { get; set; }
    }

    public class FillColor
    {
        public int b { get; set; }
        public int g { get; set; }
        public int r { get; set; }
    }

    public class GdVersion
    {
        public int build { get; set; }
        public int major { get; set; }
        public int minor { get; set; }
        public int revision { get; set; }
    }

    public class Hovered
    {
        public int bottomMargin { get; set; }
        public int height { get; set; }
        public int leftMargin { get; set; }
        public int rightMargin { get; set; }
        public string texture { get; set; }
        public bool tiled { get; set; }
        public int topMargin { get; set; }
        public int width { get; set; }
    }

    public class Idle
    {
        public int bottomMargin { get; set; }
        public int height { get; set; }
        public int leftMargin { get; set; }
        public int rightMargin { get; set; }
        public string texture { get; set; }
        public bool tiled { get; set; }
        public int topMargin { get; set; }
        public int width { get; set; }
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

    public class Label
    {
        public bool bold { get; set; }
        public bool italic { get; set; }
        public bool smoothed { get; set; }
        public bool underlined { get; set; }
        public string @string { get; set; }
        public string font { get; set; }
        public string textAlignment { get; set; }
        public int characterSize { get; set; }
        public Color color { get; set; }
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
        public UiSettings uiSettings { get; set; }
        public List<ObjectsGroup> objectsGroups { get; set; }
        public List<object> variables { get; set; }
        public List<Instance> instances { get; set; }
        public List<Object> objects { get; set; }
        public List<Event> events { get; set; }
        public List<Layer> layers { get; set; }
        public List<BehaviorsSharedDatum> behaviorsSharedData { get; set; }
    }

    public class LoadingScreen
    {
        public int backgroundColor { get; set; }
        public double backgroundFadeInDuration { get; set; }
        public string backgroundImageResourceName { get; set; }
        public string gdevelopLogoStyle { get; set; }
        public double logoAndProgressFadeInDuration { get; set; }
        public double logoAndProgressLogoFadeInDelay { get; set; }
        public int minDuration { get; set; }
        public int progressBarColor { get; set; }
        public int progressBarHeight { get; set; }
        public int progressBarMaxWidth { get; set; }
        public int progressBarMinWidth { get; set; }
        public int progressBarWidthPercent { get; set; }
        public bool showGDevelopSplash { get; set; }
        public bool showProgressBar { get; set; }
    }

    public class Object
    {
        public string assetStoreId { get; set; }
        public string name { get; set; }
        public string tags { get; set; }
        public string type { get; set; }
        public List<object> variables { get; set; }
        public List<object> effects { get; set; }
        public List<Behavior> behaviors { get; set; }
        public int fillOpacity { get; set; }
        public int outlineSize { get; set; }
        public int outlineOpacity { get; set; }
        public FillColor fillColor { get; set; }
        public OutlineColor outlineColor { get; set; }
        public bool absoluteCoordinates { get; set; }
        public bool clearBetweenFrames { get; set; }
        public string antialiasing { get; set; }
        public int? bottomMargin { get; set; }
        public int? height { get; set; }
        public int? leftMargin { get; set; }
        public int? rightMargin { get; set; }
        public string texture { get; set; }
        public bool? tiled { get; set; }
        public int? topMargin { get; set; }
        public int? width { get; set; }
        public bool adaptCollisionMaskAutomatically { get; set; }
        public bool updateIfNotVisible { get; set; }
        public List<Animation> animations { get; set; }
        public bool? bold { get; set; }
        public bool? italic { get; set; }
        public bool? smoothed { get; set; }
        public bool? underlined { get; set; }
        public string @string { get; set; }
        public string font { get; set; }
        public string textAlignment { get; set; }
        public int? characterSize { get; set; }
        public Color color { get; set; }
        public Content content { get; set; }
        public ChildrenContent childrenContent { get; set; }
        public bool? additive { get; set; }
        public bool? destroyWhenNoParticles { get; set; }
        public int? emitterAngleA { get; set; }
        public int? emitterAngleB { get; set; }
        public int? emitterForceMax { get; set; }
        public int? emitterForceMin { get; set; }
        public int? flow { get; set; }
        public int? jumpForwardInTimeOnCreation { get; set; }
        public int? maxParticleNb { get; set; }
        public int? particleAlpha1 { get; set; }
        public int? particleAlpha2 { get; set; }
        public int? particleAlphaRandomness1 { get; set; }
        public int? particleAlphaRandomness2 { get; set; }
        public int? particleAngle1 { get; set; }
        public int? particleAngle2 { get; set; }
        public int? particleAngleRandomness1 { get; set; }
        public int? particleAngleRandomness2 { get; set; }
        public int? particleBlue1 { get; set; }
        public int? particleBlue2 { get; set; }
        public int? particleGravityX { get; set; }
        public int? particleGravityY { get; set; }
        public int? particleGreen1 { get; set; }
        public int? particleGreen2 { get; set; }
        public int? particleLifeTimeMax { get; set; }
        public double? particleLifeTimeMin { get; set; }
        public int? particleRed1 { get; set; }
        public int? particleRed2 { get; set; }
        public int? particleSize1 { get; set; }
        public int? particleSize2 { get; set; }
        public int? particleSizeRandomness1 { get; set; }
        public int? particleSizeRandomness2 { get; set; }
        public int? rendererParam1 { get; set; }
        public int? rendererParam2 { get; set; }
        public string rendererType { get; set; }
        public int? tank { get; set; }
        public string textureParticleName { get; set; }
        public int? zoneRadius { get; set; }
    }

    public class ObjectGroup
    {
        public string name { get; set; }
        public List<Object> objects { get; set; }
    }

    public class ObjectsGroup
    {
        public string name { get; set; }
        public List<Object> objects { get; set; }
    }

    public class Origin
    {
        public string identifier { get; set; }
        public string name { get; set; }
    }

    public class OriginPoint
    {
        public string name { get; set; }
        public double x { get; set; }
        public double y { get; set; }
    }

    public class OutlineColor
    {
        public int b { get; set; }
        public int g { get; set; }
        public int r { get; set; }
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

    public class Platform
    {
        public string name { get; set; }
    }

    public class PlatformSpecificAssets
    {
        // [JsonProperty("android-icon-144")]
        // public string androidicon144 { get; set; }
        //
        // [JsonProperty("android-icon-192")]
        // public string androidicon192 { get; set; }
        //
        // [JsonProperty("android-icon-36")]
        // public string androidicon36 { get; set; }
        //
        // [JsonProperty("android-icon-48")]
        // public string androidicon48 { get; set; }
        //
        // [JsonProperty("android-icon-72")]
        // public string androidicon72 { get; set; }
        //
        // [JsonProperty("android-icon-96")]
        // public string androidicon96 { get; set; }
        //
        // [JsonProperty("android-windowSplashScreenAnimatedIcon")]
        // public string androidwindowSplashScreenAnimatedIcon { get; set; }
        //
        // [JsonProperty("desktop-icon-512")]
        // public string desktopicon512 { get; set; }
        //
        // [JsonProperty("ios-icon-100")]
        // public string iosicon100 { get; set; }
        //
        // [JsonProperty("ios-icon-1024")]
        // public string iosicon1024 { get; set; }
        //
        // [JsonProperty("ios-icon-114")]
        // public string iosicon114 { get; set; }
        //
        // [JsonProperty("ios-icon-120")]
        // public string iosicon120 { get; set; }
        //
        // [JsonProperty("ios-icon-144")]
        // public string iosicon144 { get; set; }
        //
        // [JsonProperty("ios-icon-152")]
        // public string iosicon152 { get; set; }
        //
        // [JsonProperty("ios-icon-167")]
        // public string iosicon167 { get; set; }
        //
        // [JsonProperty("ios-icon-180")]
        // public string iosicon180 { get; set; }
        //
        // [JsonProperty("ios-icon-20")]
        // public string iosicon20 { get; set; }
        //
        // [JsonProperty("ios-icon-29")]
        // public string iosicon29 { get; set; }
        //
        // [JsonProperty("ios-icon-40")]
        // public string iosicon40 { get; set; }
        //
        // [JsonProperty("ios-icon-50")]
        // public string iosicon50 { get; set; }
        //
        // [JsonProperty("ios-icon-57")]
        // public string iosicon57 { get; set; }
        //
        // [JsonProperty("ios-icon-58")]
        // public string iosicon58 { get; set; }
        //
        // [JsonProperty("ios-icon-60")]
        // public string iosicon60 { get; set; }
        //
        // [JsonProperty("ios-icon-72")]
        // public string iosicon72 { get; set; }
        //
        // [JsonProperty("ios-icon-76")]
        // public string iosicon76 { get; set; }
        //
        // [JsonProperty("ios-icon-80")]
        // public string iosicon80 { get; set; }
        //
        // [JsonProperty("ios-icon-87")]
        // public string iosicon87 { get; set; }
        //
        // [JsonProperty("liluo-thumbnail")]
        // public string liluothumbnail { get; set; }
    }

    public class Point
    {
        public string name { get; set; }
        public double x { get; set; }
        public double y { get; set; }
    }

    public class Pressed
    {
        public int bottomMargin { get; set; }
        public int height { get; set; }
        public int leftMargin { get; set; }
        public int rightMargin { get; set; }
        public string texture { get; set; }
        public bool tiled { get; set; }
        public int topMargin { get; set; }
        public int width { get; set; }
    }

    public class Properties
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
        public PlatformSpecificAssets platformSpecificAssets { get; set; }
        public LoadingScreen loadingScreen { get; set; }
        public Watermark watermark { get; set; }
        public List<object> authorIds { get; set; }
        public List<object> authorUsernames { get; set; }
        public List<string> categories { get; set; }
        public List<string> playableDevices { get; set; }
        public List<object> extensionProperties { get; set; }
        public List<Platform> platforms { get; set; }
        public string currentPlatform { get; set; }
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
        public Origin origin { get; set; }
        public List<Resource> resources { get; set; }
        public List<object> resourceFolders { get; set; }
    }

    public class Root
    {
        public string firstLayout { get; set; }
        public GdVersion gdVersion { get; set; }
        public Properties properties { get; set; }
        public List<Resource> resources { get; set; }
        public List<Object> objects { get; set; }
        public List<object> objectsGroups { get; set; }
        public List<object> variables { get; set; }
        public List<Layout> layouts { get; set; }
        public List<object> externalEvents { get; set; }
        public List<EventsFunctionsExtension> eventsFunctionsExtensions { get; set; }
        public List<object> externalLayouts { get; set; }
        public List<object> externalSourceFiles { get; set; }
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

    public class Sprite
    {
        public bool hasCustomCollisionMask { get; set; }
        public string image { get; set; }
        public List<Point> points { get; set; }
        public OriginPoint originPoint { get; set; }
        public CenterPoint centerPoint { get; set; }
        public List<List<object>> customCollisionMask { get; set; }
    }

    public class StringParameters
    {
        public string color { get; set; }
        public string groundColor { get; set; }
        public string skyColor { get; set; }
        public string top { get; set; }
    }

    public class SubInstruction
    {
        public Type type { get; set; }
        public List<string> parameters { get; set; }
    }

    public class Thumb
    {
        public bool adaptCollisionMaskAutomatically { get; set; }
        public bool updateIfNotVisible { get; set; }
        public List<Animation> animations { get; set; }
    }

    public class Type
    {
        public string value { get; set; }
        public bool? inverted { get; set; }
    }

    public class UiSettings
    {
        public bool grid { get; set; }
        public string gridType { get; set; }
        public int gridWidth { get; set; }
        public int gridHeight { get; set; }
        public int gridOffsetX { get; set; }
        public int gridOffsetY { get; set; }
        public int gridColor { get; set; }
        public double gridAlpha { get; set; }
        public bool snap { get; set; }
        public double zoomFactor { get; set; }
        public bool windowMask { get; set; }
    }

    public class Variable
    {
        public bool folded { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Watermark
    {
        public string placement { get; set; }
        public bool showWatermark { get; set; }
    }

    public class WhileCondition
    {
        public Type type { get; set; }
        public List<string> parameters { get; set; }
    }
}