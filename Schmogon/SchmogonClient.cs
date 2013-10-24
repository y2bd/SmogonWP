using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schmogon
{
  public partial class SchmogonClient : ISchmogonClient
  {
    private const string SitePrefix = "http://www.smogon.com";

    private const string DescHeader = "Description";
    private const string CompHeader = "Competitive Use";
  }
}
