using System;
using Remotion.Mixins;
using Remotion.Mixins.UnitTests.SampleTypes;

[assembly: Mix (typeof (TargetClassForGlobalMix), typeof (MixinForGlobalMix),
    AdditionalDependencies = new Type[] {typeof (AdditionalDependencyForGlobalMix)},
    SuppressedMixins = new Type[] {typeof (SuppressedMixinForGlobalMix)})]

[assembly: Mix (typeof (TargetClassForGlobalMix), typeof (SuppressedMixinForGlobalMix))]
[assembly: Mix (typeof (TargetClassForGlobalMix), typeof (AdditionalDependencyForGlobalMix))]