using System;
using System.Data;
using System.Data.Entity.Validation;

namespace Trident.Data.EntityFramework.Common
{
    public static class VerifySourceOfDataException
    {
        public static bool IsDuplicateEntity(DataException ex)
        {
            Exception current = ex;
            do
            {
                if (current.Message.Contains("UNIQUE KEY") || current.Message.Contains("duplicate"))
                    return true;
                current = current.InnerException;
            } while (current != null);

            return false;
        }

        public static bool IsValidEntity(DbEntityValidationException ex)
        {
            var result = false;
            if (ex.EntityValidationErrors == null) return result;
            foreach (var validationResult in ex.EntityValidationErrors)
            {
                result = !validationResult.IsValid;
            }
            return result;
        }
    }
}
