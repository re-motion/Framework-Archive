using System;
using System.Reflection.Emit;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.CodeGeneration;

using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Remotion.CodeGeneration.DPExtensions;
using System.Reflection;

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