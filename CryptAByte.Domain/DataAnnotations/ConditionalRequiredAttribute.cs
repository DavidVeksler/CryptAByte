using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace CryptAByte.Domain.DataAnnotations
{
    /*** TODO: Add client side validation support */
    public sealed class ConditionalRequiredAttribute : RequiredAttribute
    {
        public ConditionalRequiredAttribute(Expression<Func<dynamic, bool>> predicate)
        {
            this.Predicate = predicate;
        }

        public Expression<Func<dynamic, bool>> Predicate { get; set; }
        private Func<dynamic, bool> Func { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new NotSupportedException();
            }

            this.SetupFunc();

            // if condition doesn't match, validation success.
            if (!this.Func.Invoke(validationContext.ObjectInstance))
            {
                return ValidationResult.Success;
            }

            // if the condition matches, the value is required
            return base.IsValid(value, validationContext);
        }

        private void SetupFunc()
        {
            if (this.Func == null)
            {
                if (this.Predicate == null)
                {
                    throw new InvalidOperationException("The predicate must be set to a valid Boolean expression.");
                }
                this.Func = this.Predicate.Compile();
            }
        }
    }
}
