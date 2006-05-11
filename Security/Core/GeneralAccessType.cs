using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data;

namespace Rubicon.Security
{
  [AccessType]
  public enum GeneralAccessType
  {
    [PermanentGuid ("1D6D25BC-4E85-43ab-A42D-FB5A829C30D5")]
    Create = 0,
    [PermanentGuid ("62DFCD92-A480-4d57-95F1-28C0F5996B3A")]
    Read = 1,
    [PermanentGuid ("11186122-6DE0-4194-B434-9979230C41FD")]
    Edit = 2,
    [PermanentGuid ("305FBB40-75C8-423a-84B2-B26EA9E7CAE7")]
    Delete = 3,
    [PermanentGuid ("203B7478-96F1-4bf1-B4EA-5BDD1206252C")]
    Find = 4
  }
}
