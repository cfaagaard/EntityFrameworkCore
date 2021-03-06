﻿using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Microsoft.EntityFrameworkCore.ModelBuilding
{
    public abstract partial class ModelBuilderTest
    {
        public class IQueryTypeConfigurationTest
        {
            [Fact]
            public void Configure_query_not_already_in_model()
            {
                var builder = InMemoryTestHelpers.Instance.CreateConventionBuilder();

                builder.ApplyConfiguration(new CustomerConfiguration());

                var entityType = builder.Model.FindEntityType(typeof(Customer));
                Assert.NotNull(entityType);
            }

            [Fact]
            public void Configure_query_already_in_model()
            {
                var builder = InMemoryTestHelpers.Instance.CreateConventionBuilder();

                builder.Query<Customer>();
                builder.ApplyConfiguration(new CustomerConfiguration());

                var entityType = builder.Model.FindEntityType(typeof(Customer));
                Assert.Equal(200, entityType.FindProperty(nameof(Customer.Name)).GetMaxLength());
            }

            [Fact]
            public void Override_config_in_query_type_configuration()
            {
                var builder = InMemoryTestHelpers.Instance.CreateConventionBuilder();

                builder.Query<Customer>().Property(c => c.Name).HasMaxLength(500);
                builder.ApplyConfiguration(new CustomerConfiguration());

                var entityType = builder.Model.FindEntityType(typeof(Customer));
                Assert.Equal(200, entityType.FindProperty(nameof(Customer.Name)).GetMaxLength());
            }

            [Fact]
            public void Override_config_after_query_type_configuration()
            {
                var builder = InMemoryTestHelpers.Instance.CreateConventionBuilder();

                builder.ApplyConfiguration(new CustomerConfiguration());
                builder.Query<Customer>().Property(c => c.Name).HasMaxLength(500);

                var entityType = builder.Model.FindEntityType(typeof(Customer));
                Assert.Equal(500, entityType.FindProperty(nameof(Customer.Name)).GetMaxLength());
            }

            [Fact]
            public void Apply_multiple_query_type_configurations()
            {
                var builder = InMemoryTestHelpers.Instance.CreateConventionBuilder();

                builder.ApplyConfiguration(new CustomerConfiguration());
                builder.ApplyConfiguration(new CustomerConfiguration2());

                var entityType = builder.Model.FindEntityType(typeof(Customer));
                Assert.Equal(1000, entityType.FindProperty(nameof(Customer.Name)).GetMaxLength());
            }

            private class CustomerConfiguration : IQueryTypeConfiguration<Customer>
            {
                public void Configure(QueryTypeBuilder<Customer> builder)
                {
                    builder.Property(c => c.Name).HasMaxLength(200);
                }
            }

            private class CustomerConfiguration2 : IQueryTypeConfiguration<Customer>
            {
                public void Configure(QueryTypeBuilder<Customer> builder)
                {
                    builder.Property(c => c.Name).HasMaxLength(1000);
                }
            }
        }
    }
}
