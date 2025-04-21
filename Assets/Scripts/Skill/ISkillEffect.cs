public interface ISkillEffect {
    void Apply(ref int damage, CharaData attacker, CharaData defender);
}