using System;
using System.Collections.Generic;
using System.Text;

namespace RobloxClicker.Models;

public class Achievement
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long Reward { get; set; }
    public bool IsCompleted { get; set; } = false;
    public string ConditionType { get; set; } = string.Empty;
    public long TargetValue { get; set; }
}
