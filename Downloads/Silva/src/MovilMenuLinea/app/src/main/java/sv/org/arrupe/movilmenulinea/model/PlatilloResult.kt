package sv.org.arrupe.retrofitapi.model

import com.google.gson.annotations.SerializedName

data class PlatilloResult(
    @SerializedName("platillos")
    val platillos: List<Platillo>
)