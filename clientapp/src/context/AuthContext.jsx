import React, { createContext, useContext, useState, useEffect } from 'react';
import axios from 'axios';

const AuthContext = createContext();

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(() => {
        const savedUser = localStorage.getItem('user');
        return savedUser ? JSON.parse(savedUser) : null;
    });

    const login = async (email, password) => {
        console.log("Intentando login con:", email);

        const response = await axios.post('http://localhost:5016/api/auth/login', {
            email,
            password
        });

        console.log("Respuesta login completa JSON:", response.data);
        const { token } = response.data;

        if (!token) {
            throw new Error("Token no recibido");
        }

        // Decodificar el token manualmente (si no recibes user directamente)
        const payloadBase64 = token.split('.')[1];
        const decoded = JSON.parse(atob(payloadBase64));
        const role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        const name = decoded["FullName"];
        const userId = decoded["sub"];

        const userData = { name, role, userId };
        console.log("Usuario decodificado:", userData);

        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(userData));
        setUser(userData);

        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    };

    const logout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        setUser(null);
        delete axios.defaults.headers.common['Authorization'];
    };

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        }
    }, []);

    return (
        <AuthContext.Provider value={{ user, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};
