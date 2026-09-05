package sv.org.arrupe.retrofitapi.model

import com.google.gson.annotations.SerializedName

data class Platillo(
    @SerializedName("idPlatillo")
    val idPlatillo: Int,

    @SerializedName("nombre")
    val nombre: String?,

    @SerializedName("descripcion")
    val descripcion: String?,

    @SerializedName("precio")
    val precio: Double,

    @SerializedName("imagenUrl")
    val imagenUrl: String?,

    @SerializedName("tiempoPreparacion")
    val tiempoPreparacion: Int?,

    @SerializedName("estado")
    val estado: String?,

    @SerializedName("idCategoria")
    val idCategoria: Int?,

    @SerializedName("categoriaNombre")
    val categoriaNombre: String?,

    @SerializedName("ingredientes")
    val ingredientes: List<String> = emptyList()
)