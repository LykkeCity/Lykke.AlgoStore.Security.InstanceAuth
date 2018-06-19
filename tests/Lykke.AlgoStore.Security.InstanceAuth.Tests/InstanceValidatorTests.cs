using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using NUnit.Framework;
using System;

namespace Lykke.AlgoStore.Security.InstanceAuth.Tests
{
    [TestFixture]
    public class InstanceValidatorTests
    {
        [Test]
        public void Validate_ThrowsArgumentNull_WhenInstanceDataNull()
        {
            Assert.Throws<ArgumentNullException>(() => InstanceValidator.Validate(null));
        }

        [Test]
        public void Validate_ReturnsFalse_WhenInstanceNotStarted()
        {
            var data = new AlgoClientInstanceData
            {
                AlgoInstanceStatus = CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceStatus.Deploying
            };

            Assert.IsFalse(InstanceValidator.Validate(data));
        }

        [Test]
        public void Validate_ReturnsTrue_WhenInstanceStarted()
        {
            var data = new AlgoClientInstanceData
            {
                AlgoInstanceStatus = CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceStatus.Started
            };

            Assert.IsTrue(InstanceValidator.Validate(data));
        }
    }
}
