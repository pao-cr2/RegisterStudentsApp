// src/services/teacherService.js
const API_URL = 'https://localhost:5001/api/teachers'; // Ajusta al puerto y ruta de tu API

export async function getTeachers() {
    const response = await fetch(API_URL);
    if (!response.ok) throw new Error('Error al obtener profesores');
    return await response.json();
}
