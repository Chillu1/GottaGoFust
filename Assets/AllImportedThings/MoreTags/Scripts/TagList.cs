namespace MoreTags
{
    public static class Tag
    {
        public static AllTags all = new AllTags();
        public static ObjectGroup Object = new ObjectGroup("Object");
        public static ColorGroup Color = new ColorGroup("Color");
    }

    public class ObjectGroup : TagGroup
    {
        public ObjectGroup(string name) : base(name) { }
        public TagName Cube = new TagName("Object.Cube");
        public TagName Sphere = new TagName("Object.Sphere");
        public TagName Capsule = new TagName("Object.Capsule");
        public TagName Cylinder = new TagName("Object.Cylinder");
    }

    public class ColorGroup : TagGroup
    {
        public ColorGroup(string name) : base(name) { }
        public TagName Red = new TagName("Color.Red");
        public TagName Green = new TagName("Color.Green");
        public TagName Blue = new TagName("Color.Blue");
        public TagName Yellow = new TagName("Color.Yellow");
        public TagName Cyan = new TagName("Color.Cyan");
        public TagName Magenta = new TagName("Color.Magenta");
    }

    public class AllTags : TagNames
    {
        public AllTags() : base(TagSystem.AllTags()) { }
        public TagChildren Cube = new TagChildren("Cube");
        public TagChildren Sphere = new TagChildren("Sphere");
        public TagChildren Capsule = new TagChildren("Capsule");
        public TagChildren Cylinder = new TagChildren("Cylinder");
        public TagChildren Red = new TagChildren("Red");
        public TagChildren Green = new TagChildren("Green");
        public TagChildren Blue = new TagChildren("Blue");
        public TagChildren Yellow = new TagChildren("Yellow");
        public TagChildren Cyan = new TagChildren("Cyan");
        public TagChildren Magenta = new TagChildren("Magenta");
    }
}
