namespace Trident.UI.Blazor.Components.Buttons
{

    public enum BsButtonType
    {
        Primary,
        Secondary,
        Danger,
        Link,
        Default
    }

    public static class BsButtonTypeExtensions
    {
        public static string ToCssClass(this BsButtonType buttonType)
        {
            return $"btn-{buttonType.ToString().ToLowerInvariant()}";
        }
    }
}
