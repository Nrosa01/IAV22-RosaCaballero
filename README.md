# Proyeco Final de IAV - Nicolás Rosa Caballero
## Parasite - IA Adaptativa en runtime

## Indice
- [Proyeco Final de IAV - Nicolás Rosa Caballero](#proyeco-final-de-iav---nicolás-rosa-caballero)
  - [Parasite - IA Adaptativa en runtime](#parasite---ia-adaptativa-en-runtime)
  - [Indice](#indice)
- [Descripción del proyecto](#descripción-del-proyecto)
- [Punto de partida](#punto-de-partida)
- [Paquetes de Unity añadidos](#paquetes-de-unity-añadidos)
- [Funcioamiento de la IA](#funcioamiento-de-la-ia)
- [Pseudocodigo](#pseudocodigo)
- [Controles](#controles)

# Descripción del proyecto

El proyecto consiste en un prototipo de un juego de acción en tercera persona con elemento de shooters "twin-stick" y un ritmo rápido fuertemente inspirado en __No More Heroes__ y __Furi__. Al igual que este último este juego consiste en una boss rush en espacios limitados. El Gimmick de este juego recae en una mecánica e IA únicas. Por un lado, tanto nuestro personaje como el enemigo tienen el mismo set de acciones, moverse, ataque a melee, ataque de rango, habilidad de movimiento y habilidad especial, similar a una "ulti". Lo característico es que estas habilidades pueden cambiar en tiempo de ejecución, en concreto las del jugador. Este, al recibir daño es "corrompido" por el enemigo y sus habilidades cambian a otras similares a las de este. Por otro lado, el enemigo usará una IA adaptativa que reaccione a estos cambios y buscará patrones más óptimos para ganar.

La IA del enemigo además deberá permitir configuraciones de tendencias, podría ser más agresiva, más cauta... Estas "restricciones" forzarán  la IA a salir de la ruta más óptima para ganar a cambio de tener más personalidad a la vez que tratarán de compensar sus debilidades lo máximo posible. Cuando los ataques del jugador cambian, el rango y daño de estos pueden cambiar, lo cual también influye en la IA.

Por cuestiones técnicas y de tiempo no habrá obstáculos en el mapa, pero de haberlos la IA debería poder sortearlos sin necesidad de un navmesh usando sensores y la información que recibe de ellos.

# Punto de partida

Aunque la parte más relevante del proyecto sea la IA del enemigo, he considero crear un proyecto base del juego con una arquitectura decente para facilitarme la tarea posterior. El punto de partida es un proyecto vacío de Unity 2021.2.10f1 con los paquetes que menciono en el apartado siguiente. Tras eso he creado un pequeño sistema modular de acciones que funciona tanto para el jugado como el enemigo, además de una cámara que trata de mantener a ambos personajes visibles al mismo tiempo que trata de estar cerca. El funcioamiento de la cámara es similar al de __Furi__. El sistema de acciones ha sido implementado por mi desde 0, es flexible, soporta cooldown, es cancelable (una acción puede ser interrumpida, por ejemplo por recibir daño o por un QTE). Además también soporta acciones enlazadas, por ejemplo si tu ataque a melee tiene 3 partes, solo vas a ejecutar la 2 si ha pasado muy poco tiempo desde que se ejecutó la 1. Podría decirse que es un sistema de combos simplificado donde solo existe un combo por tipo de acción, pero lo considero suficiente para el alcance de este proyecto y del juego, más complejidad no es necesaria.

A nivel de gráficos uso un Shader Toon que realicé siguiendo un tutorial en YouTube, por lo que no estoy seguro de si cuenta como recurso propio. Como el juego tiene pocas luces y transcurre en un espacio pequeño he optado por realizarlo para el render en modo forward.

# Paquetes de Unity añadidos

Para la realización del proyecto, he importado los siguientes paquetes:

- Mathematics
  - Paquete de matemáticos necesario para usar mis librerías de matemáticas.
- TextMeshPro
  - Paquete para la creación de textos eficientes usado en la UI y menús.
- URP
  - Paquete usado para implementar una SRP con soporte de Shadergraph. Me facilita crear un apartado visual decente y ligero para prototipar mejor. Junto con este paquete instalé el de Post Processing.
- Input System
  - Me permite desacoplar el Input de las acciones del jugador y tener una mejor arquitectura.

- Unitask
  - Este es el único paquete que no es oficial de Unity. Uso para crear una tarea asíncrona en la que se ejecuta una función cada cierto tiempo GC friendly. Es una alternativa a las corrutinas de Unity muy solvente y que considero obligatoria en cualquier proyecto, hasta el punto de que es el primer paquete que añado.
- RioniUtils
  - Paquete propio que contiene una colección de scripts con métodos de extensión y funciones matemáticas que me ayudan a trabajar con los datos de Unity.

# Funcioamiento de la IA

En este apartdo voy a describir el funcioamineto de la IA del enemigo. Este apartado es similar al pseudocodigo que se mostrará en el siguiente, sin embargo en este apartado nos basaremos en el concepto de la IA de forma que cualquier persona pueda entender que es lo que hace, mientras que en el siguiente lo que veremos será un prototipo de posible implementación.

#  Pseudocodigo

El pseudocodigo de este proyecto puede llegar a ser extenso, por lo cual lo he dividido por partes para facilitar la lectura. Estas partes están en archivos separados a los que se accede a través de los links que pondré a continuación. Cabe mencionar que el estilo de Pseudocodigo que he elegido es uno propio, tratando de que se parezca a C# y usando su nomenclatura. Considero que si el proyecto va a ser escrito en C#, hacer que el pseudocodigo sea similar y use características propias del lenguaje facilita la integración. El pseudocodigo tendrá anotaciones en Español, pero los comentarios del código en sí serán en inglés, ya que me gusta desarrollar en inglés.

 - [Pseudocodigo del buffer de acciones](ActionSystemPseudo.md)
 - [Pseudocodigo del sistema de IA](IASystemPseudo.md)

# Controles

En este apartado se describirá como se controla el juego. Cabe destacar que aunque el juego soporta gamepad o teclado y ratón, es recomendable jugarlo con mando ya que ha sido diseñado para ser jugado con con un gamepad, además, si la disposición de botones es tipo XBOX es aún más cómod que si la disposición es tipo PS4/PS5.

