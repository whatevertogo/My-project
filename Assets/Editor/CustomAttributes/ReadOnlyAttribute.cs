using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
    public bool IsReadOnly { get; private set; } = true;

    // 无参数构造函数
    public ReadOnlyAttribute()
    {
        IsReadOnly = true;
    }

    // 可选：提供带参数的构造函数
    public ReadOnlyAttribute(bool isReadOnly)
    {
        IsReadOnly = isReadOnly;
    }
}
