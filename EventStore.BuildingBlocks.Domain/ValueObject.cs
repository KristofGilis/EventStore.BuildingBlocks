using System.Collections;
using System.Reflection;

namespace EventStore.BuildingBlocks.Domain;

public abstract class ValueObject<T>
        where T : ValueObject<T>
{
    private static readonly Member[] Members = GetMembers().ToArray();

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return obj.GetType() == typeof(T) && Members.All(m =>
        {
            var otherValue = m.GetValue(obj);
            var thisValue = m.GetValue(this);
            return m.IsNonStringEnumerable
                ? GetEnumerableValues(otherValue).SequenceEqual(GetEnumerableValues(thisValue))
                : otherValue?.Equals(thisValue) ?? otherValue == thisValue;
        });
    }

    public override int GetHashCode() =>
        CombineHashCodes(
            Members.Select(m => m.IsNonStringEnumerable
                ? CombineHashCodes(GetEnumerableValues(m.GetValue(this)))
                : m.GetValue(this)));

    private static IEnumerable<Member> GetMembers()
    {
        var t = typeof(T);
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        while (t != typeof(object))
        {
            if (t == null)
                continue;
            foreach (var p in t.GetProperties(flags))
                yield return new Member(p);
            foreach (var f in t.GetFields(flags))
                yield return new Member(f);
            t = t.BaseType;
        }
    }

    private static IEnumerable<object> GetEnumerableValues(object obj)
    {
        var enumerator = ((IEnumerable)obj).GetEnumerator();
        while (enumerator.MoveNext())
            yield return enumerator.Current!;
    }

    private static int CombineHashCodes(IEnumerable<object> objs)
    {
        unchecked
        {
            return objs.Aggregate(17, (current, obj) => current * 59 + (obj?.GetHashCode() ?? 0));
        }
    }

    private struct Member
    {
        public readonly Func<object, object> GetValue;
        public readonly bool IsNonStringEnumerable;

        public Member(MemberInfo info)
        {
            switch (info)
            {
                case FieldInfo field:
                    GetValue = obj => field.GetValue(obj)!;
                    IsNonStringEnumerable = typeof(IEnumerable).IsAssignableFrom(field.FieldType) &&
                                            field.FieldType != typeof(string);
                    break;
                case PropertyInfo prop:
                    GetValue = obj => prop.GetValue(obj)!;
                    IsNonStringEnumerable = typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) &&
                                            prop.PropertyType != typeof(string);
                    break;
                default:
                    throw new ArgumentException("Member is not a field or property?!?!", info.Name);
            }
        }
    }
}
