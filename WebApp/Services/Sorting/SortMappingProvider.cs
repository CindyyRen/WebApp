namespace WebApp.Services.Sorting;

public sealed class SortMappingProvider(IEnumerable<ISortMappingDefinition> sortMappingDefinitions)
{
    public bool ValidateMappings<TSource, TDestination>(string? sort)
    {
        //接受传过来的字符串，如果为空，就返回true
        if (string.IsNullOrWhiteSpace(sort))
        {
            return true;
        }
        //拿到第一个逗号分隔的字段，然后拿到第一个空格前面的字段名，最后过滤掉空白字段，组成一个列表
        var sortFields = sort
            .Split(',')
            .Select(f => f.Trim().Split(' ')[0])
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .ToList();

        SortMapping[] mapping = GetMappings<TSource, TDestination>();

        return sortFields.All(f => mapping.Any(m => m.SortField.Equals(f, StringComparison.OrdinalIgnoreCase)));
    }
    public SortMapping[] GetMappings<TSource, TDestination>()
    {
        //sortMappingDefinition是controller里传入的(query.Sort)
        SortMappingDefinition<TSource, TDestination>? sortMappingDefinition = sortMappingDefinitions
              .OfType<SortMappingDefinition<TSource, TDestination>>()
            .FirstOrDefault();

        if (sortMappingDefinition is null)
        {
            throw new InvalidOperationException(
                $"The mapping from '{typeof(TSource).Name}' into'{typeof(TDestination).Name} isn't defined");
        }

        return sortMappingDefinition.Mappings;
    }
}

