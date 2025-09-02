using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class CustomFieldTests : BaseEqualityTests<CustomField>
{
    protected override CustomField CreateSampleInstance()
    {
        return new CustomField
        {
            Id = 1,
            Name = "Test Field",
            CustomizedType = "issue",
            FieldFormat = "string",
            Regexp = "",
            MinLength = 0,
            MaxLength = 100,
            IsRequired = false,
            IsFilter = true,
            Searchable = true,
            Multiple = false,
            DefaultValue = "default",
            Visible = true,
            PossibleValues = [new CustomFieldPossibleValue { Value = "value1", Label = "Label 1" }]
        };
    }

    protected override CustomField CreateDifferentInstance()
    {
        var field = CreateSampleInstance();
        field.Name = "Different Field";
        return field;
    }
}