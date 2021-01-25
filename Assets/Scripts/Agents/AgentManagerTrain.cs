using UnityEngine;

namespace MBaske
{
    public class AgentManagerTrain : AgentManager
    {
        private const float m_Radius = 10;

        protected override void OnHealthDepleted(FighterAgent agent) 
        {
            // Reward opponents in vicinity.
            AddTeamReward(agent, true, 1);
            // Penalize team members in vicinity.
            AddTeamReward(agent, false, -1f);
            
            // Back to default pos, episode continues.
            agent.ResetAgent();
        }

        // Team rewards are supposed to encourage team work, like pack-hunting.
        // Although I havent't seen such behaviour emerging yet.
        protected void AddTeamReward(FighterAgent agent, bool opponents, float reward)
        {
            var team = TeamAgents(agent, opponents, m_Radius);

            foreach (var other in team)
            {
                other.AddReward(reward);
            }
        }
    }
}