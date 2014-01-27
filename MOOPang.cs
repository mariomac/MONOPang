/*------------------------------------------------------------------------------
 * Este código está distribuido bajo una licencia del tipo BEER-WARE.
 * -----------------------------------------------------------------------------
 * Mario Macías Lloret escribió este archivo. Teniendo esto en cuenta,
 * puedes hacer lo que quieras con él: modificarlo, redistribuirlo, venderlo,
 * etc, aunque siempre deberás indicar la autoría original en tu código.
 * Además, si algún día nos encontramos por la calle y piensas que este código
 * te ha sido de utilidad, estás obligado a invitarme a una cerveza (a ser
 * posible, de las buenas) como recompensa por mi contribución.
 * -----------------------------------------------------------------------------
 */
package moo.pang;

import moo.pang.multimedia.Juego;

/**
 * Esta clase no hace nada más que alojar el método main(), y mantener continuamente
 * el ciclo de la aplicación, que es: presentación - partida - mensaje de fin de juego
 * A pesar de que este ciclo parece que vaya a prolongarse hasta el infinito, debido
 * a que está dentro de un "while(true)", el hecho es que la clase Ventana está
 * preparada (yo he decidido que sea así en el código interno) para que cuando se cierre
 * la ventana mediante la cruz de cerrar o mediante la tecla ESC, el programa se termine.
 *
 * @author Mario Macías: http://mario.site.ac.upc.edu
 */
public class MOOPang {
    private MOOPang() {}

    /**
     * Mantiene
     * el ciclo de la aplicación, que es: presentación - partida - mensaje de fin de juego
     * A pesar de que este ciclo parece que vaya a prolongarse hasta el infinito, debido
     * a que está dentro de un "while(true)", el hecho es que la clase Ventana está
     * preparada (yo he decidido que sea así en el código interno) para que cuando se cierre
     * la ventana mediante la cruz de cerrar o mediante la tecla ESC, el programa se termine.
     * @param args
     */
    public static void main(String[] args) {
        // Se crea el objeto "Juego"
        Juego elJuego = new Juego();
        //Repite infinitamente (cuando el usuario cierre la ventana, internamente
        //se llamará a System.exit() y se saldrá de este bucle)...
        while(true) {
            //Se muestra la presentación
            elJuego.presentacion();
            //Cuando se sale de la presentación, empieza la partida
            elJuego.partida();
            //cuando se acaba la partida, se muestra el mensaje de fin de juego
            elJuego.finDeJuego();
        }

    }
}
