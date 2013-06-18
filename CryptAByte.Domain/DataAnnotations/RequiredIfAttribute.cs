using System;
using System.ComponentModel.DataAnnotations;

namespace CryptAByte.Domain.DataAnnotations
{
    //http://www.devtrends.co.uk/blog/the-complete-guide-to-validation-in-asp.net-mvc-3-part-2
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RequiredIfAttribute : ValidationAttribute
    {
        public string OtherProperty { get; private set; }
        public Comparison Comparison { get; private set; }
        public object Value { get; private set; }
        private const string DefaultErrorMessageFormatString = "The {0} field is required.";

        public RequiredIfAttribute(string otherProperty, Comparison comparison, object value)
        {
            if (string.IsNullOrEmpty(otherProperty))
                throw new ArgumentNullException("otherProperty");

            OtherProperty = otherProperty;
            Comparison = comparison;
            Value = value;
            ErrorMessage = DefaultErrorMessageFormatString;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                var otherProperty = validationContext.ObjectType.GetProperty(OtherProperty);
                var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);

                if (ConditionIsMet(otherPropertyValue))
                    return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
        
        private bool ConditionIsMet(object actualPropertyValue)
        {
            var areEqual = actualPropertyValue != null && actualPropertyValue.Equals(Value);
            return Comparison == Comparison.IsEqualTo ? areEqual : !areEqual;
        }
    }

    public enum Comparison
    {
        IsEqualTo,
        IsNotEqualTo
    }
}
