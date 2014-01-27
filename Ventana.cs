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
package moo.multimedia;

import java.awt.Color;
import java.awt.Font;
import java.awt.Graphics;
import java.awt.Image;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.awt.event.WindowEvent;
import java.awt.event.WindowListener;
import java.awt.image.BufferedImage;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.swing.JFrame;
import javax.swing.JPanel;

/**
 * Esta clase oculta toda la complejidad que hay detrás de la creación de ventanas,
 * lectura de las teclas de control, dibujo de primitivas gráficas, etc... La mayor
 * parte del código de esta clase es algo difícil de entender sólo con los conocimientos
 * de teoría de la asignatura MOO. La gracia de la programación Orientada a Objetos es
 * que no necesitas entenderlo para poder usar esta clase; simplemente puedes limitarte
 * a entender qué hacen sus métodos públicos y constructores. Si eres un machacas
 * y quieres aprender cosas para tu propia satisfacción, siéntete libre de investigar
 * cada línea de código, intentar modificarlo o ampliarlo y ver qué pasa.
 *
 * El modo de funcionamiento del dibujo es el siguiente: todo lo que se dibuje 
 * (mediante las funciones escribeTexto, dibujaCirculo, etc...), se irá dibujando en un lienzo oculto.
 * Una vez se ha acabado de mostrar el lienzo, se llamará al método "mostrarLienzo",
 * que mostrará el lienzo en la pantalla.
 *
 * OJO! Las coordenadas que se utilizan son <b>coordenadas de pantalla</b>. Eso quiere
 * decir que el origen (0,0) está en la esquina superior izquierda de la ventana.
 * Coordenadas más altas de x estarán más a la derecha de la ventana, y coordenadas
 * más altas de y estarán situadas más hacia abajo (estas últimas van al contrario
 * de las coordenadas Y que se suelen utilizar en las gráficas matemáticas).
 *
 * @author Mario Macías http://mario.site.ac.upc.edu
 */
public class Ventana implements KeyListener, WindowListener {
    /**
     * Indica el número de fotogramas por segundo. Es decir, el máximo de veces
     * que se puede mostrar el lienzo por pantalla en un segundo.
     */
    private static final float FOTOGRAMAS_SEGUNDO = 25;

    /**
     * Guarda una imagen del lienzo en el que se irán pintando las cosas
     */
    private Image lienzo;
    /**
     * Graphics es una clase a través de la cual podremos dibujar en el lienzo.
     */
    private Graphics fg;

    /**
     * JFrame es un objeto que maneja una ventana de pantalla.
     */
    private JFrame marcoVentana = null;

    /**
     * Las siguientes variables booleanas guardan el estado de algunas teclas.
     * serán "true" cuando alguna de estas teclas esté pulsada, y "false" en caso
     * contrario.
     */
    private boolean teclaArriba = false,
            teclaAbajo = false,
            teclaIzquierda = false,
            teclaDerecha = false,
            barraEspaciadora = false;

    /**
     * Abre una nueva ventana.
     * <b>NOTA</b>: Si se cierra la ventana con el ratón, el programa acabará.
     * @param titulo El texto que aparecerá en la barra de título de la ventana.
     * @param ancho Anchura de la ventana en píxels
     * @param alto Altura de la ventana en píxels
     */
    public Ventana(String titulo, int ancho, int alto) {
        final JPanel pantalla = new JPanel(true);
        marcoVentana = new JFrame(titulo) {
            @Override
            public void paint(Graphics g) {
                //super.paint(g);
                pantalla.getGraphics().drawImage(lienzo, 0, 0, null);
            }
        };
        marcoVentana.setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
        marcoVentana.addWindowListener(this);
        marcoVentana.setSize(ancho, alto);
        marcoVentana.setResizable(false);
        marcoVentana.getContentPane().add(pantalla);
        marcoVentana.setVisible(true);
        lienzo = new BufferedImage(pantalla.getWidth(), pantalla.getHeight(), BufferedImage.TYPE_INT_RGB);
        fg = lienzo.getGraphics();
        borrarLienzoOculto();
        marcoVentana.addKeyListener(this);

    }

    /**
     * Comprueba si la flecha "Arriba" del cursor está pulsada o no.
     * @return true si está pulsada. false en caso contrario.
     */
    public boolean isPulsadoArriba() {
        return teclaArriba;
    }
    /**
     * Comprueba si la flecha "Abajo" del cursor está pulsada o no.
     * @return true si está pulsada. false en caso contrario.
     */
    public boolean isPulsadoAbajo() {
        return teclaAbajo;
    }
    /**
     * Comprueba si la flecha "Izquierda" del cursor está pulsada o no.
     * @return true si está pulsada. false en caso contrario.
     */
    public boolean isPulsadoIzquierda() {
        return teclaIzquierda;
    }
    /**
     * Comprueba si la flecha "Derecha" del cursor está pulsada o no.
     * @return true si está pulsada. false en caso contrario.
     */
    public boolean isPulsadoDerecha() {
        return teclaDerecha;
    }
    /**
     * Comprueba si la barra espaciadora está pulsada o no.
     * <b>NOTA:</b> a diferencia de los cursores, la barra espaciadora debe
     * soltarse y volver a pulsarse para que la función devuelva "true" dos veces.
     * @return true si está pulsada. false en caso contrario.
     */
    public boolean isPulsadoEspacio() {
        if(barraEspaciadora) {
            barraEspaciadora = false;
            return true;
        } else {
            return false;
        }
    }

    /**
     * Cierra la ventana.
     */
    public void cerrar() {
        marcoVentana.dispose();
        System.exit(0);
    }

    private int ultimoTamanyo = 0;
    private Font ultimaFuente = new Font(Font.SANS_SERIF,Font.PLAIN,12);;
    /**
     * Escribe un texto por pantalla.
     * @param texto El texto a escribir.
     * @param x Coordenada izquierda del inicio del texto.
     * @param y Coordenada superior del inicio del texto.
     * @param medidaFuente Tamaño de la fuente, en píxels.
     * @param color Color del texto.
     */
    public void escribeTexto(String texto, float x, float y, int medidaFuente, Color color) {
        fg.setColor(color);
        if(ultimoTamanyo != medidaFuente) {
            ultimaFuente = new Font(Font.SANS_SERIF,Font.PLAIN,medidaFuente);
        }
        fg.setFont(ultimaFuente);
        fg.drawString(texto, (int)x, (int)y);
    }

    private int[] xTriangle = new int[3];
    private int[] yTriangle = new int[3];
    /**
     * Dibuja un triángulo, dadas tres coordenadas en píxeles y un color.
     * @param x1,y1 Coordenadas x,y del primer punto.
     * @param x2,y2 Coordenadas x,y del segundo punto.
     * @param x3,y3 Coordenadas x,y del tercer punto.
     * @param color Color del triángulo.
     */
    public void dibujaTriangulo(float x1, float y1, float x2, float y2, float x3, float y3, Color color){
        xTriangle[0] = (int)x1; xTriangle[1] = (int)x2; xTriangle[2] = (int)x3;
        yTriangle[0] = (int)y1; yTriangle[1] = (int)y2; yTriangle[2] = (int)y3;
        fg.setColor(color);
        fg.fillPolygon(xTriangle,yTriangle,3);
    }

    /**
     * Dibuja un rectángulo en pantalla, dadas las coordenadas de su esquina superior izquierda,
     * su anchura y su altura.
     * 
     * @param izquierda Coordenada del lado más a la izquierda del rectángulo.
     * @param arriba Coordenada del lado superior del rectángulo.
     * @param ancho Anchura del rectángulo, en pixels.
     * @param alto Altura del rectángulo, en píxels.
     * @param color Color del rectángulo.
     */
    public void dibujaRectangulo(float izquierda, float arriba, float ancho, float alto, Color color) {
        fg.setColor(color);
        fg.fillRect((int)izquierda, (int)arriba, (int)ancho, (int)alto);
    }

    /**
     * Dibuja un círculo por pantalla.
     * @param centroX Coordenada X del centro del círculo (en píxels).
     * @param centroY Coordenada Y del centro del círculo (en píxels).
     * @param radio Radio del círculo, en píxels.
     * @param color Color del círculo.
     */
    public void dibujaCirculo(float centroX, float centroY, float radio, Color color) {
        fg.setColor(color);
        fg.fillArc((int)(centroX - radio), (int)(centroY - radio), (int)(radio*2f),(int)(radio*2f) , 0, 360);
    }

    /**
     * Borra el contenido del lienzo oculto (lo deja todo de color negro)
     */
    public void borrarLienzoOculto() {
        fg.setColor(Color.black);
        fg.fillRect(0, 0, (int)getAnchuraLienzo(), (int)getAlturaLienzo());
    }

    /**
     * Devuelve la anchura del lienzo, en píxels.
     * @return La anchura del lienzo.
     */
    public float getAnchuraLienzo() {
        return lienzo.getWidth(null);
    }

    /**
     * Devuelve la altura del lienzo, en píxels.
     * @return La altura del lienzo.
     */
    public float getAlturaLienzo() {
        return lienzo.getHeight(null);
    }

    private long lastFrameTime = 0;
    /**
     * Muestra el contenido (dibujo) del lienzo oculto por pantalla.
     */
    public void mostrarLienzo() {
        marcoVentana.repaint();

        // Para que no vaya más rápido en ordenadores muy rápidos, y más lento
        // en ordenadores lentos, se hace que vaya siempre a la misma velocidad
        // limitando el número de fotogramas por segundo (especificado en la constante
        // FOTOGRAMAS_SEGUNDO). Cuando se ejecuta y se repinta todo, se hace que el
        // programa se ponga en estado de "sueño" (método Thread.sleep()) durante un
        // tiempo en el que el programa estará parado. Así nos aseguramos que
        // Siempre tendremos limitado el número de fotogramas por segundo al valor
        // que especifica la constante FOTOGRAMAS_SEGUNDO
        long now = System.currentTimeMillis();
        try {
            float sleepTime = (1000.0f / FOTOGRAMAS_SEGUNDO) - (float)(now - lastFrameTime);
            if(sleepTime <= 0) {
                Thread.yield();
            } else {
                Thread.sleep((int)sleepTime);
            }
        } catch (InterruptedException ex) {
            Logger.getLogger(Ventana.class.getName()).log(Level.SEVERE, null, ex);
        }
        lastFrameTime = System.currentTimeMillis();
    }

    /**
     * Implementación de los métodos de la interfaz WindowListener.
     * De esta manera, la clase recibe llamadas cuando a la ventana
     * le sucede algún "evento". A nosotros solamente nos interesa el
     * método "windowClosing", que se llama cuando se pulsa la cruz
     * de cerrar en el marco superior de la ventana.
     * Previamente se ha tenido que añadir una instancia de esta clase
     * mediante el método addWindowListener() de JFrame.
     */
        public void windowClosed(WindowEvent e) {}
        public void windowActivated(WindowEvent e) {}
        public void windowClosing(WindowEvent e) {
            cerrar();
        }
        public void windowDeactivated(WindowEvent e) {}
        public void windowDeiconified(WindowEvent e) {}
        public void windowIconified(WindowEvent e) {}
        public void windowOpened(WindowEvent e) {}

    /**
     * Implementacion de los métodos relativos a la interfaz KeyListener
     * Cuando se llama al evento keyPressed, ponemos como "true" la tecla pulsada.
     * Cuando se llama al evento keyReleased, ponemos como "false" la tecla pulsada.
     * Previamente se ha tenido que añadir una instancia de esta clase
     * mediante el método addKeyListener() de JFrame.
     */
        private boolean spaceReleased = true;

        public void keyTyped(KeyEvent e) {
        }

        public void keyPressed(KeyEvent e) {
            switch(e.getKeyCode()) {
                case KeyEvent.VK_UP:
                    teclaArriba = true;
                    break;
                case KeyEvent.VK_DOWN:
                    teclaAbajo = true;
                    break;
                case KeyEvent.VK_LEFT:
                    teclaIzquierda = true;
                    break;
                case KeyEvent.VK_RIGHT:
                    teclaDerecha = true;
                    break;
                case KeyEvent.VK_SPACE:
                    if (spaceReleased) {
                        barraEspaciadora = true;
                    }
                    spaceReleased = false;
                    break;
                case KeyEvent.VK_ESCAPE:
                    cerrar();

            }
        }

        public void keyReleased(KeyEvent e) {
            switch(e.getKeyCode()) {
                case KeyEvent.VK_UP:
                    teclaArriba = false;
                    break;
                case KeyEvent.VK_DOWN:
                    teclaAbajo = false;
                    break;
                case KeyEvent.VK_LEFT:
                    teclaIzquierda = false;
                    break;
                case KeyEvent.VK_RIGHT:
                    teclaDerecha = false;
                    break;
                case KeyEvent.VK_SPACE:
                    spaceReleased = true;
                    barraEspaciadora = false;
                    break;
            }
        }
}
