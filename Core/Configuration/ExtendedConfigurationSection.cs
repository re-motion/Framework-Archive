using System;
using System.Configuration;

namespace Rubicon.Configuration
{
  public abstract class ExtendedConfigurationSection: ConfigurationSection
  {
    // constants

    // types

    // static members

    // member fields

    // construction and disposing

    protected ExtendedConfigurationSection()
    {
    }

    // methods and properties

    protected internal new object this [ConfigurationProperty property]
    {
      get { return base[property]; }
      set { base[property] = value; }
    }
  }
}