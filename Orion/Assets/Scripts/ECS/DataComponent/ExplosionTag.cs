
using Unity.Entities;


// C'est un component vide que l'on va coller à nos projectiles uniquement quand il y aura collision avec un joueur
// Le principe est le suivant: à chaque frame on a un system qui delete les entities qui ont le composant ExplosionTag
public struct ExplosionTag : IComponentData
{}
