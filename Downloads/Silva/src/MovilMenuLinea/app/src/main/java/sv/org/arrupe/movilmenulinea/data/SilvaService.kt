package sv.org.arrupe.retrofitapi.data

import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.http.GET
import sv.org.arrupe.retrofitapi.model.Platillo

interface SilvaService {
    @GET("api/platillos")
    suspend fun listPlatillos(): List<Platillo>
}

object SilvaServiceFactory {
    private const val BASE_URL = "http://192.168.1.8:5195/"

    fun makeSilvaService(): SilvaService {
        return Retrofit.Builder()
            .baseUrl(BASE_URL)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(SilvaService::class.java)
    }
}