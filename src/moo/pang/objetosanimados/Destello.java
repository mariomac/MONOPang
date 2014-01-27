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

package moo.pang.objetosanimados;

import java.awt.Color;
import java.util.Random;
import moo.multimedia.Ventana;
import moo.pang.multimedia.Juego;

/**
 * Clase que implementa los destellos que aparecen cuando una bola es explotada.
 *
 * @author Mario Macías: http://mario.site.ac.upc.edu
 */
public class Destello implements ObjetoAnimado {
    /**
     * Radio máximo (en píxels) que alcanza el destello antes de desaparecer.
     */
    private static float RADIO_MAXIMO = 10;
    /**
     * Posicion en pantalla (en píxels) del destello.
     */
    private float centroX, centroy;
    /**
     * Radio que en cada fotograma tiene el destello (va creciendo).
     */
    private float radio;

    private Juego elJuego;

    /**
     * Crea un destello en una posición determinada.
     * @param centroX Coordenada X en pantalla (en píxels) del centro del destello.
     * @param centroY Coordenada Y en pantalla (en píxels) del centro del destello.
     * @param elJuego Referencia al Juego donde el destello está ocurriendo.
     */
    public Destello(float centroX, float centroY, Juego elJuego) {
        this.centroX = centroX;
        this.centroy = centroY;
        this.elJuego = elJuego;
        radio = 3;
    }
    /**
     * Objeto Random utilizado para crear colores aleatorios en cada fotograma.
     */
    private static Random RND = new Random();

    /**
     * Implementación del método moverYDibujar de la interfaz ObjetoAnimado.
     * Simplemente incrementa un poco el radio en cada fotograma y dibuja el
     * destello. Cuando el radio del destello alcanza una determinada medida,
     * se elimina de la lista de objetos animados.
     * @param v
     */
    public void moverYDibujar(Ventana v) {
        radio++;
        if(radio > RADIO_MAXIMO) {
            elJuego.eliminarObjetoAnimado(this);
        } else {
            Color colorAleatorio =  new Color(RND.nextFloat(), RND.nextFloat(), RND.nextFloat());
            v.dibujaCirculo(centroX, centroy, radio,colorAleatorio);
            v.dibujaCirculo(centroX, centroy, radio-3, Color.black);
        }
    }


}
