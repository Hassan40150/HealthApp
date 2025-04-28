using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public class RequiredIfAttribute : ValidationAttribute
{
    private readonly string _dependentProperty;

    public RequiredIfAttribute(string dependentProperty)
    {
        _dependentProperty = dependentProperty;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var dependentProperty = validationContext.ObjectType.GetProperty(_dependentProperty);
        if (dependentProperty == null)
            return new ValidationResult($"Unknown property: {_dependentProperty}");

        var dependentValue = dependentProperty.GetValue(validationContext.ObjectInstance, null);

        // If the dependent property has a value
        if (dependentValue != null && !string.IsNullOrEmpty(dependentValue.ToString()))
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success!;
    }
}
