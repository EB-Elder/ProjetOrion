
using Unity.Entities;


// C'est un component vide que l'on va coller à nos joueurs et méchants uniquement quand il y aura collision et que des dégats en resulteront
// Le principe est le suivant: à chaque frame on a un system qui retire de la vie aux éléments porteurs du hittag en fonction de sa data "Damage"
public struct HitTag : IComponentData
{

    public int damage;

}
