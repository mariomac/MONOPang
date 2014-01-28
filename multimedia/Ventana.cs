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
using System.Windows.Forms;

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
using System;
using System.Drawing;
using System.Threading;


public class Ventana : Form { // implements KeyListener, WindowListener {
    /**
     * Indica el número de fotogramas por segundo. Es decir, el máximo de veces
     * que se puede mostrar el lienzo por pantalla en un segundo.
     */
	private const float FOTOGRAMAS_SEGUNDO = 25;

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
    /**
     * Las siguientes variables boolas guardan el estado de algunas teclas.
     * serán "true" cuando alguna de estas teclas esté pulsada, y "false" en caso
     * contrario.
     */
    private bool teclaArriba = false,
            teclaAbajo = false,
            teclaIzquierda = false,
            teclaDerecha = false,
            barraEspaciadora = false;

    private Point[] Triangle = new Point[3];

    /**
     * Abre una nueva ventana.
     * <b>NOTA</b>: Si se cierra la ventana con el ratón, el programa acabará.
     * @param titulo El texto que aparecerá en la barra de título de la ventana.
     * @param ancho Anchura de la ventana en píxels
     * @param alto Altura de la ventana en píxels
     */
    public Ventana (String titulo, int ancho, int alto)
	{
		for (int i = 0; i < Triangle.Length; i++) {
			Triangle[i] = new Point();
		}
		base.Text = titulo;
		this.Paint += new PaintEventHandler(pintar);
		this.FormClosed += new FormClosedEventHandler(cerrar);
		this.Size = new Size(ancho,alto);
		this.FormBorderStyle = FormBorderStyle.FixedSingle;
		lienzo = new Bitmap(ancho,alto);
		fg = Graphics.FromImage(lienzo);
        borrarLienzoOculto();
		this.KeyDown += new KeyEventHandler(KeyDownH);
		this.KeyUp += new KeyEventHandler(KeyUpH);
    }

	public void pintar(object sender, PaintEventArgs pea) {
		pea.Graphics.DrawImageUnscaled(lienzo,0,0);
	}

    /**
     * Comprueba si la flecha "Arriba" del cursor está pulsada o no.
     * @return true si está pulsada. false en caso contrario.
     */
    public bool isPulsadoArriba() {
        return teclaArriba;
    }
    /**
     * Comprueba si la flecha "Abajo" del cursor está pulsada o no.
     * @return true si está pulsada. false en caso contrario.
     */
    public bool isPulsadoAbajo() {
        return teclaAbajo;
    }
    /**
     * Comprueba si la flecha "Izquierda" del cursor está pulsada o no.
     * @return true si está pulsada. false en caso contrario.
     */
    public bool isPulsadoIzquierda() {
        return teclaIzquierda;
    }
    /**
     * Comprueba si la flecha "Derecha" del cursor está pulsada o no.
     * @return true si está pulsada. false en caso contrario.
     */
    public bool isPulsadoDerecha() {
        return teclaDerecha;
    }
    /**
     * Comprueba si la barra espaciadora está pulsada o no.
     * <b>NOTA:</b> a diferencia de los cursores, la barra espaciadora debe
     * soltarse y volver a pulsarse para que la función devuelva "true" dos veces.
     * @return true si está pulsada. false en caso contrario.
     */
    public bool isPulsadoEspacio() {
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
    public void cerrar(object sender, FormClosedEventArgs fceh) {
		this.Dispose(true);
		Application.Exit();
		Environment.Exit(0);
    }

    private int ultimoTamanyo = 0;
	private Font ultimaFuente = new Font(FontFamily.GenericSansSerif,12,GraphicsUnit.Pixel);
    /**
     * Escribe un texto por pantalla.
     * @param texto El texto a escribir.
     * @param x Coordenada izquierda del inicio del texto.
     * @param y Coordenada superior del inicio del texto.
     * @param medidaFuente Tamaño de la fuente, en píxels.
     * @param color Color del texto.
     */
    public void escribeTexto(String texto, float x, float y, int medidaFuente, Color color) {        
        if(ultimoTamanyo != medidaFuente) {
            ultimaFuente = new Font(FontFamily.GenericSansSerif,medidaFuente,GraphicsUnit.Pixel);
        }
        //fg.setFont(ultimaFuente);
		Brush b = new SolidBrush(color);
		fg.DrawString(texto,ultimaFuente,b,x,y);
    }

    /**
     * Dibuja un triángulo, dadas tres coordenadas en píxeles y un color.
     * @param x1,y1 Coordenadas x,y del primer punto.
     * @param x2,y2 Coordenadas x,y del segundo punto.
     * @param x3,y3 Coordenadas x,y del tercer punto.
     * @param color Color del triángulo.
     */
    public void dibujaTriangulo(float x1, float y1, float x2, float y2, float x3, float y3, Color color){
        Triangle[0].X = (int)x1; Triangle[1].X = (int)x2; Triangle[2].X = (int)x3;
        Triangle[0].Y = (int)y1; Triangle[1].Y = (int)y2; Triangle[2].Y = (int)y3;
		Brush b = new SolidBrush(color);
		fg.FillPolygon(b,Triangle);
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
		Brush b = new SolidBrush(color);
		fg.FillRectangle(b,(int)izquierda, (int)arriba, (int)ancho, (int)alto);
    }

    /**
     * Dibuja un círculo por pantalla.
     * @param centroX Coordenada X del centro del círculo (en píxels).
     * @param centroY Coordenada Y del centro del círculo (en píxels).
     * @param radio Radio del círculo, en píxels.
     * @param color Color del círculo.
     */
    public void dibujaCirculo(float centroX, float centroY, float radio, Color color) {
		Brush b = new SolidBrush(color);
		fg.FillEllipse(b,(int)(centroX - radio), (int)(centroY - radio), (int)(radio*2f),(int)(radio*2f));
    }

    /**
     * Borra el contenido del lienzo oculto (lo deja todo de color negro)
     */
    public void borrarLienzoOculto() {
		Brush b = new SolidBrush(Color.Black);
        fg.FillRectangle(b,0, 0, (int)getAnchuraLienzo(), (int)getAlturaLienzo());
    }

    /**
     * Devuelve la anchura del lienzo, en píxels.
     * @return La anchura del lienzo.
     */
    public float getAnchuraLienzo() {
		return lienzo.Width;
    }

    /**
     * Devuelve la altura del lienzo, en píxels.
     * @return La altura del lienzo.
     */
    public float getAlturaLienzo() {
		return lienzo.Height;
    }

    private DateTime lastFrameTime = DateTime.UtcNow;
    /**
     * Muestra el contenido (dibujo) del lienzo oculto por pantalla.
     */
    public void mostrarLienzo() {
		this.Refresh();

        // Para que no vaya más rápido en ordenadores muy rápidos, y más lento
        // en ordenadores lentos, se hace que vaya siempre a la misma velocidad
        // limitando el número de fotogramas por segundo (especificado en la constante
        // FOTOGRAMAS_SEGUNDO). Cuando se ejecuta y se repinta todo, se hace que el
        // programa se ponga en estado de "sueño" (método Thread.sleep()) durante un
        // tiempo en el que el programa estará parado. Así nos aseguramos que
        // Siempre tendremos limitado el número de fotogramas por segundo al valor
        // que especifica la constante FOTOGRAMAS_SEGUNDO
		DateTime now = DateTime.UtcNow;
        float sleepTime = (1000.0f / FOTOGRAMAS_SEGUNDO) - (float)(now - lastFrameTime).TotalMilliseconds;
        if(sleepTime <= 0) {
			Thread.Yield();
        } else {
            Thread.Sleep((int)sleepTime);
        }
        lastFrameTime = DateTime.UtcNow;
    }



    /**
     * Implementacion de los métodos relativos a la interfaz KeyListener
     * Cuando se llama al evento keyPressed, ponemos como "true" la tecla pulsada.
     * Cuando se llama al evento keyReleased, ponemos como "false" la tecla pulsada.
     * Previamente se ha tenido que añadir una instancia de esta clase
     * mediante el método addKeyListener() de JFrame.
     */
    private bool spaceReleased = true;


    public void KeyDownH(object sender, KeyEventArgs ev) {
		if(ev.KeyCode == Keys.Up ) {
			teclaArriba = true;
		} else if(ev.KeyCode == Keys.Down ) {
			teclaAbajo = true;
		} else if(ev.KeyCode == Keys.Left ) {
			teclaIzquierda = true;
		} else if(ev.KeyCode == Keys.Right ) {
			teclaDerecha = true;
		} else if(ev.KeyCode == Keys.Space ) {
			if(spaceReleased) {
				barraEspaciadora = true;
			}
			spaceReleased = false;
		} else if(ev.KeyCode == Keys.Escape ) {
			cerrar (null,null);
		}

    }

        public void KeyUpH(object sender, KeyEventArgs ev) {
            switch(ev.KeyCode) {
				case Keys.Up:
                    teclaArriba = false;
                    break;
				case Keys.Down:
                    teclaAbajo = false;
                    break;
				case Keys.Left:
                    teclaIzquierda = false;
                    break;
				case Keys.Right:
                    teclaDerecha = false;
                    break;
				case Keys.Space:
                    spaceReleased = true;
                    barraEspaciadora = false;
                    break;
            }
        }
}
