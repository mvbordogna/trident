using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trident.Business;
using Trident.TestTargetProject.Domain;
using Trident.Validation;

namespace Trident.TestTargetProject.Business.Test.ValidationRules
{
    public class OrgValidtionDemoRule : ValidationRuleBase<BusinessContext<Organisation>>
    {
        public override int RunOrder => 1;

        public override Task Run(BusinessContext<Organisation> context, List<ValidationResult> errors)
        {
            Console.WriteLine("rule run: OrgValidtionDemo2Rule");
            return Task.CompletedTask;
        }
    }
}
