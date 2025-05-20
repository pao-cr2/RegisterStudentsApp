import React from 'react';
import { useAuth } from '../context/AuthContext';

const Home = () => {
    const { user } = useAuth();

    return (
        <div style={{ padding: 20 }}>
            <h1>Bienvenido</h1>
            <p>Usuario: {user?.name}</p>
            <p>Rol: {user?.role}</p>
        </div>
    );
};

export default Home;
