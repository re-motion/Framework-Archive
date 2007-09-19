using System;
using System.Reflection.Emit;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.CodeGeneration;

using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Rubicon.CodeGeneration.DPExtensions;
using System.Reflection;

namespace Rubicon.Core.UnitTests.CodeGeneration
{
  [TestFixture]
  public class AttributeReplicatorTest
  {
    [Test]
    public void ReplicateAttribute ()
    {
      MockRepository mockRepository = new MockRepository ();
      IAttributableEmitter emitter = mockRepository.CreateMock<IAttributableEmitter> ();
      
      // expect
      emitter.AddCustomAttribute (null);
      LastCall.Constraints (Mocks_Is.NotNull ());

      mockRepository.ReplayAll ();

      CustomAttributeData data = CustomAttributeData.GetCustomAttributes (typeof (AttributeReplicatorTest))[0];
      AttributeReplicator.ReplicateAttribute (emitter, data);

      mockRepository.VerifyAll ();
    }
  }
}