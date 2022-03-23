using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trident.Business;
using Trident.Samples.Domain.Entities;
using Trident.Validation;

namespace Trident.TestTargetProject.Business.Test.ValidationRules
{
    public class OrgValidtionDemoRule : ValidationRuleBase<BusinessContext<OrganizationEntity>>
    {
        public override int RunOrder => 1;

        public override Task Run(BusinessContext<OrganizationEntity> context, List<ValidationResult> errors)
        {
            Console.WriteLine("rule run: OrgValidtionDemo2Rule");
            return Task.CompletedTask;
        }
    }
}
