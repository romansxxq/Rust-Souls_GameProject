using System.Collections.Generic;

/// <summary>
/// Тип кімнати відповідно до GDD Rust &amp; Souls.
/// </summary>
public enum RoomType
{
    Start,   // стартова кімната забігу
    Combat,  // бойова сутичка — дає Rust/Souls
    Elite,   // елітний ворог — підвищений ризик, краща нагорода
    Event,   // випадкова подія з вибором і ризиком
    Shop,    // магазин: купівля перків/зброї за Rust
    Rest,    // відпочинок: відновлення HP
    Boss     // фінальний бос локації
}

/// <summary>
/// Вузол графу забігу. Зберігає тип кімнати та посилання на наступні вузли.
/// Гравець бачить next і обирає, в яку кімнату йти.
/// </summary>
[System.Serializable]
public class RoomNode
{
    public RoomType type;
    public bool visited;
    public bool isCurrent;
    public List<RoomNode> next = new List<RoomNode>();
}
