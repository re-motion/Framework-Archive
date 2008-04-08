using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.CodeGeneration;
using Remotion.CodeGeneration.DPExtensions;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.Core.UnitTests.CodeGeneration
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