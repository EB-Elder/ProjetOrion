import gym
import numpy as np

#Initialisation de l'environnement
env = gym.make("MountainCar-v0")



#Taux d'apprentissage
LEARNING_RATE = 0.1
# "Poids" des nouvelles récompenses comparés aux anciennes
DISCOUNT = 0.95
#Epochs
EPISODES = 25000

#Coefficient d'exploration
EPSILON = 0.5

START_EPSILON_DECAYING = 1
END_EPSILON_DECAYING = EPISODES // 2

epsilon_decay_value = EPSILON/(END_EPSILON_DECAYING - START_EPSILON_DECAYING)

SHOW_EVERY = 2000

#Création du tableau qui contiendra les valeurs discrètes
DISCRETE_OS_SIZE = [20] * len(env.observation_space.high)

#Étant donné qu'il existe une infinité d'état (valeur d'observation dite continue =/= discrètes) on va crée des chunks de valeurs
discrete_os_win_size = (env.observation_space.high - env.observation_space.low) / DISCRETE_OS_SIZE

#Création du Tableau qui contiendra les combinaison d'observation en fonction des récompenses
q_table = np.random.uniform(low=-2, high=0, size=(DISCRETE_OS_SIZE + [env.action_space.n]))

def get_discrete_state(state):
    discrete_state = (state - env.observation_space.low)/discrete_os_win_size
    print(tuple(discrete_state.astype(np.int)))
    return tuple(discrete_state.astype(np.int))


for episode in range(EPISODES):

    if episode % SHOW_EVERY == 0:
        render = True
    else:
        render = False
    discrete_state = get_discrete_state(env.reset())
    done = False


    while not done:

        if np.random.random() > EPSILON:
            action = np.argmax(q_table[discrete_state])
        else:
            action = np.random.randint(0, env.action_space.n)
        new_state, reward, done, _ = env.step(action)

        new_discrete_state = get_discrete_state(new_state)

        if render:
            env.render()
        if not done:
            #Récupéré le choix d'actions ou la récompense est la plus élevé
            max_future_q = np.max(q_table[new_discrete_state])
            current_q = q_table[discrete_state + (action, )]

            #Formule du Q Learning
            new_q = (1 - LEARNING_RATE) * current_q + LEARNING_RATE * (reward + DISCOUNT * max_future_q)

            q_table[discrete_state + (action, )] = new_q
        elif new_state[0] >= env.goal_position:
            print(f"We made it on episode {episode}")
            q_table[discrete_state + (action, )] = 0

        discrete_state = new_discrete_state

    if END_EPSILON_DECAYING >= episode >= START_EPSILON_DECAYING:
        EPSILON -= epsilon_decay_value

env.close()