/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSQL
 *
 * THIS MATERIAL IS PROVIDED AS IS, WITH ABSOLUTELY NO WARRANTY EXPRESSED
 * OR IMPLIED.  ANY USE IS AT YOUR OWN RISK.
 * 
 * Permission is hereby granted to use or copy this program
 * for any purpose,  provided the above notices are retained on all copies.
 * Permission to modify the code and to distribute modified code is granted,
 * provided the above notices are retained, and a notice that the code was
 * modified is included with the above copyright notice.
 *
 */

using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class FbValueGenerationStrategyConvention : DatabaseGeneratedAttributeConvention, IModelInitializedConvention
    {
        public override InternalPropertyBuilder Apply(InternalPropertyBuilder propertyBuilder, DatabaseGeneratedAttribute attribute, MemberInfo clrMember)
        {
            FbValueGenerationStrategy? valueGenerationStrategy = null;
            ValueGenerated valueGenerated = ValueGenerated.Never;
            if (attribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Computed)
            {
                valueGenerated = ValueGenerated.OnAddOrUpdate;
                valueGenerationStrategy = FbValueGenerationStrategy.ComputedColumn;
            }
            else if (attribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
            {
                valueGenerated = ValueGenerated.OnAdd;
                valueGenerationStrategy = FbValueGenerationStrategy.IdentityColumn;
            }

            propertyBuilder.ValueGenerated(valueGenerated, ConfigurationSource.Convention);
            propertyBuilder.Firebird(ConfigurationSource.DataAnnotation).ValueGenerationStrategy(valueGenerationStrategy);

            return base.Apply(propertyBuilder, attribute, clrMember);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalModelBuilder Apply(InternalModelBuilder modelBuilder)
        {
            modelBuilder.Firebird(ConfigurationSource.Convention).ValueGenerationStrategy(FbValueGenerationStrategy.IdentityColumn);

            return modelBuilder;
        }
    }
}
