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

import moo.multimedia.Ventana;

/**
 * Interfaz que describe todos los métodos que debe implementar cualquier objeto
 * que quiera ser movido por el videojuego y pintado en pantalla.
 *
 * @author Mario Macías: http://mario.site.ac.upc.edu
 */
public interface ObjetoAnimado {
    /**
     * Este método se llama para cada fotograma del juego. En él, el objeto
     * que implemente esta interfaz deberá moverse un paso en relación al
     * fotograma anterior, y redibujarse por la ventana.
     * 
     * @param ventana Ventana donde se dibujará el objeto animado.
     */
    public void moverYDibujar(Ventana v);
}
